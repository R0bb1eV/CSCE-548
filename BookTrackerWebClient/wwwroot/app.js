const panels = document.querySelectorAll(".panel");
const apiBaseUrlInput = document.getElementById("apiBaseUrl");

initializeApiBaseUrl(apiBaseUrlInput);

for (const panel of panels) {
    const entity = panel.dataset.entity;
    const output = panel.querySelector('[data-role="output"]');

    panel.querySelector('[data-action="all"]').addEventListener("click", async (event) => {
        await runAction(event, output, () => handleGetAll(entity, output));
    });

    panel.querySelector('[data-action="single"]').addEventListener("click", async (event) => {
        const idText = panel.querySelector('[data-role="id-input"]').value.trim();
        await runAction(event, output, () => handleGetSingle(entity, idText, output));
    });

    panel.querySelector('[data-action="subset"]').addEventListener("click", async (event) => {
        const field = panel.querySelector('[data-role="filter-field"]').value.trim();
        const value = panel.querySelector('[data-role="filter-value"]').value.trim();
        await runAction(event, output, () => handleGetSubset(entity, field, value, output));
    });
}

function initializeApiBaseUrl(input) {
    const cached = localStorage.getItem("apiBaseUrl");
    if (cached) {
        input.value = cached;
    }

    input.addEventListener("change", () => {
        const cleaned = normalizeBaseUrl(input.value);
        input.value = cleaned;
        localStorage.setItem("apiBaseUrl", cleaned);
    });
}

function normalizeBaseUrl(value) {
    return value.trim().replace(/\/$/, "");
}

function getApiBaseUrl() {
    return normalizeBaseUrl(apiBaseUrlInput.value);
}

async function runAction(event, output, action) {
    const button = event.currentTarget;
    const originalText = button.textContent;
    button.disabled = true;
    button.textContent = "Loading...";

    try {
        await action();
    } finally {
        button.disabled = false;
        button.textContent = originalText;
    }
}

async function handleGetAll(entity, output) {
    renderMessage(output, `Loading ${toTitle(entity)}...`);

    const response = await apiRequest({
        method: "GET",
        path: `/api/${entity}`
    });

    if (!response.ok) {
        renderError(output, response);
        return;
    }

    if (!Array.isArray(response.body)) {
        renderMessage(output, "The API did not return a list. Please check the server.");
        return;
    }

    renderTable(output, response.body, response);
}

async function handleGetSingle(entity, idText, output) {
    if (!idText) {
        renderMessage(output, "Please enter an ID first.");
        return;
    }

    renderMessage(output, `Finding ${toTitle(entity)} #${idText}...`);

    const response = await apiRequest({
        method: "GET",
        path: `/api/${entity}/${encodeURIComponent(idText)}`
    });

    if (!response.ok) {
        renderError(output, response);
        return;
    }

    if (response.body && typeof response.body === "object") {
        renderTable(output, Array.isArray(response.body) ? response.body : [response.body], response);
        return;
    }

    renderMessage(output, "The API did not return a record for that ID.");
}

async function handleGetSubset(entity, field, value, output) {
    if (!field || !value) {
        renderMessage(output, "Please enter a field name and a filter value.");
        return;
    }

    renderMessage(output, `Filtering ${toTitle(entity)} by ${field}...`);

    const response = await apiRequest({
        method: "GET",
        path: `/api/${entity}`
    });

    if (!response.ok) {
        renderError(output, response);
        return;
    }

    if (!Array.isArray(response.body)) {
        renderMessage(output, "The API did not return a list for filtering.");
        return;
    }

    const subset = response.body.filter((row) => {
        const fieldValue = getCaseInsensitivePropertyValue(row, field);
        if (fieldValue === undefined || fieldValue === null) {
            return false;
        }

        return String(fieldValue).toLowerCase().includes(value.toLowerCase());
    });

    renderTable(output, subset, response, `${subset.length} matching rows`);
}

function toTitle(value) {
    return value.charAt(0).toUpperCase() + value.slice(1);
}

async function apiRequest({ method, path }) {
    const baseUrl = getApiBaseUrl();
    if (!baseUrl) {
        return buildClientError("API Base URL is required.");
    }

    let url;
    try {
        url = new URL(baseUrl + path).toString();
    } catch {
        return buildClientError(`Invalid API Base URL: "${baseUrl}".`);
    }

    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 12000);
    const startedAt = performance.now();

    try {
        const response = await fetch(url, {
            method,
            signal: controller.signal
        });

        const durationMs = Math.round(performance.now() - startedAt);
        const text = await response.text();
        const parsed = tryParseJson(text);

        return {
            ok: response.ok,
            status: response.status,
            url,
            method,
            durationMs,
            body: parsed.ok ? parsed.value : text
        };
    } catch (error) {
        const durationMs = Math.round(performance.now() - startedAt);
        return normalizeNetworkError(error, { url, method, durationMs });
    } finally {
        clearTimeout(timeoutId);
    }
}

function buildClientError(message) {
    return {
        ok: false,
        status: 0,
        url: "",
        method: "",
        durationMs: 0,
        body: { message }
    };
}

function normalizeNetworkError(error, context) {
    if (error && error.name === "AbortError") {
        return {
            ok: false,
            status: 0,
            ...context,
            body: {
                message: "Request timed out after 12 seconds.",
                hint: "Check that the API is running and the base URL is correct."
            }
        };
    }

    const message = error && error.message ? error.message : String(error);
    return {
        ok: false,
        status: 0,
        ...context,
        body: {
            message: "Network error while contacting the API.",
            details: message,
            hint: "Check the API Base URL and ensure the server is running."
        }
    };
}

function tryParseJson(text) {
    try {
        return { ok: true, value: JSON.parse(text) };
    } catch {
        return { ok: false };
    }
}

function renderMessage(output, message) {
    output.innerHTML = "";
    const wrapper = document.createElement("div");
    wrapper.className = "output-message";
    wrapper.textContent = message;
    output.appendChild(wrapper);
}

function renderError(output, response) {
    output.innerHTML = "";
    const card = document.createElement("div");
    card.className = "output-error";

    const title = document.createElement("p");
    title.className = "output-error-title";
    title.textContent = "We could not load the data.";

    const details = document.createElement("p");
    details.className = "output-error-details";
    details.textContent = response.body && response.body.message
        ? response.body.message
        : "The server returned an error.";

    const hint = document.createElement("p");
    hint.className = "output-error-hint";
    hint.textContent = "Check the API Base URL and make sure the server is running.";

    card.appendChild(title);
    card.appendChild(details);
    card.appendChild(hint);
    output.appendChild(card);
}

function renderTable(output, rows, response, overrideHeader) {
    output.innerHTML = "";

    const header = document.createElement("div");
    header.className = "output-header";
    header.textContent = overrideHeader
        ? `${overrideHeader} in ${response.durationMs} ms.`
        : `Loaded ${rows.length} rows in ${response.durationMs} ms.`;
    output.appendChild(header);

    if (rows.length === 0) {
        renderMessage(output, "No records found.");
        return;
    }

    const columns = buildColumnList(rows);
    const table = document.createElement("table");
    table.className = "data-table";

    const thead = document.createElement("thead");
    const headRow = document.createElement("tr");
    for (const column of columns) {
        const th = document.createElement("th");
        th.textContent = prettifyColumnName(column);
        headRow.appendChild(th);
    }
    thead.appendChild(headRow);
    table.appendChild(thead);

    const tbody = document.createElement("tbody");
    for (const row of rows) {
        const tr = document.createElement("tr");
        for (const column of columns) {
            const td = document.createElement("td");
            const value = row[column];
            td.textContent = formatCellValue(value);
            tr.appendChild(td);
        }
        tbody.appendChild(tr);
    }
    table.appendChild(tbody);

    output.appendChild(table);
}

function buildColumnList(rows) {
    const keys = new Set();
    for (const row of rows) {
        if (row && typeof row === "object") {
            Object.keys(row).forEach((key) => keys.add(key));
        }
    }
    return Array.from(keys);
}

function getCaseInsensitivePropertyValue(obj, key) {
    const match = Object.keys(obj).find((k) => k.toLowerCase() === key.toLowerCase());
    return match ? obj[match] : undefined;
}

function prettifyColumnName(name) {
    return name
        .replace(/([a-z])([A-Z])/g, "$1 $2")
        .replace(/_/g, " ")
        .replace(/\b\w/g, (char) => char.toUpperCase());
}

function formatCellValue(value) {
    if (value === null || value === undefined) {
        return "—";
    }

    if (typeof value === "string") {
        return value;
    }

    if (typeof value === "number" || typeof value === "boolean") {
        return String(value);
    }

    return JSON.stringify(value);
}
