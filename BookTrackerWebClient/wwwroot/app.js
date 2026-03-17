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

    panel.querySelector('[data-action="create"]').addEventListener("click", async (event) => {
        await runAction(event, output, () => handleCreate(entity, panel, output));
    });

    panel.querySelector('[data-action="update"]').addEventListener("click", async (event) => {
        await runAction(event, output, () => handleUpdate(entity, panel, output));
    });
}

function getEnvApiBaseUrl() {
    const envValues = [
        window.WEBCLIENT_API_BASE_URL,
        window.BOOKTRACKER_API_BASE_URL,
        window.NEXT_PUBLIC_API_BASE_URL,
        document.querySelector('meta[name="webclient-api-base-url"]')?.content
    ];

    for (const v of envValues) {
        if (typeof v !== "string") {
            continue;
        }

        const trimmed = v.trim();
        if (trimmed && trimmed !== "__RENDER_API_BASE_URL__") {
            return trimmed;
        }
    }

    return null;
}

function initializeApiBaseUrl(input) {
    const cached = localStorage.getItem("apiBaseUrl");
    const envUrl = getEnvApiBaseUrl();
    const initialValue = cached || envUrl || "";

    input.value = normalizeBaseUrl(initialValue);

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

async function handleCreate(entity, panel, output) {
    const parsed = collectPayload(entity, panel, false, output);

    if (!parsed.ok) {
        return;
    }

    renderMessage(output, `Creating ${toTitle(entity)}...`);

    const response = await apiRequest({
        method: "POST",
        path: `/api/${entity}`,
        body: parsed.value
    });

    if (!response.ok) {
        renderError(output, response);
        return;
    }

    if (response.body && typeof response.body === "object") {
        renderTable(output, Array.isArray(response.body) ? response.body : [response.body], response, "Created");
        return;
    }

    renderMessage(output, "Created successfully.");
}

async function handleUpdate(entity, panel, output) {
    const idText = panel.querySelector('[data-role="update-id"]').value.trim();
    if (!idText) {
        renderMessage(output, "Please enter an ID to update.");
        return;
    }

    const parsed = collectPayload(entity, panel, true, output);

    if (!parsed.ok) {
        return;
    }

    renderMessage(output, `Updating ${toTitle(entity)} #${idText}...`);

    const response = await apiRequest({
        method: "PUT",
        path: `/api/${entity}/${encodeURIComponent(idText)}`,
        body: parsed.value
    });

    if (!response.ok) {
        renderError(output, response);
        return;
    }

    if (response.status === 204) {
        renderMessage(output, `Updated ${toTitle(entity)} #${idText}.`);
        return;
    }

    if (response.body && typeof response.body === "object") {
        renderTable(output, Array.isArray(response.body) ? response.body : [response.body], response, "Updated");
        return;
    }

    renderMessage(output, `Updated ${toTitle(entity)} #${idText}.`);
}

function toTitle(value) {
    return value.charAt(0).toUpperCase() + value.slice(1);
}

async function apiRequest({ method, path, body }) {
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
    const timeoutId = setTimeout(() => controller.abort(), 25000);
    const startedAt = performance.now();

    try {
        const response = await fetch(url, {
            method,
            signal: controller.signal,
            headers: body ? { "Content-Type": "application/json" } : undefined,
            body: body ? JSON.stringify(body) : undefined
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

function collectPayload(entity, panel, isUpdate, output) {
    switch (entity) {
        case "authors":
            return collectAuthorPayload(panel, isUpdate, output);
        case "books":
            return collectBookPayload(panel, isUpdate, output);
        case "users":
            return collectUserPayload(panel, isUpdate, output);
        case "activities":
            return collectActivityPayload(panel, isUpdate, output);
        default:
            renderMessage(output, "Unsupported entity for create/update.");
            return { ok: false };
    }
}

function validateRequired(output, requiredItems) {
    const missing = requiredItems.filter((item) => item.value === null || item.value === "").map((item) => item.key);
    if (missing.length > 0) {
        renderMessage(output, `Please fill in: ${missing.join(", ")}.`);
        return false;
    }
    return true;
}

function collectBookPayload(panel, isUpdate, output) {
    const suffix = isUpdate ? "-update" : "";
    const getValue = (role) => panel.querySelector(`[data-role="${role}${suffix}"]`)?.value.trim() ?? "";
    const getNumber = (role) => {
        const raw = getValue(role);
        if (!raw) {
            return null;
        }
        const value = Number(raw);
        return Number.isFinite(value) ? value : null;
    };

    const title = getValue("book-title");
    const genre = getValue("book-genre");
    const publishingHouse = getValue("book-publisher");
    const isbn = getValue("book-isbn");
    const pageCount = getNumber("book-page-count");
    const yearOfRelease = getNumber("book-year");
    const authorId = getNumber("book-author-id");

    const required = [
        { key: "Title", value: title },
        { key: "Genre", value: genre },
        { key: "Publishing House", value: publishingHouse },
        { key: "ISBN", value: isbn },
        { key: "Page Count", value: pageCount },
        { key: "Year of Release", value: yearOfRelease },
        { key: "Author ID", value: authorId }
    ];

    if (!validateRequired(output, required)) {
        return { ok: false };
    }

    return {
        ok: true,
        value: {
            title,
            pageCount,
            genre,
            publishingHouse,
            yearOfRelease,
            isbn,
            authorId
        }
    };
}

function collectAuthorPayload(panel, isUpdate, output) {
    const suffix = isUpdate ? "-update" : "";
    const getValue = (role) => panel.querySelector(`[data-role="${role}${suffix}"]`)?.value.trim() ?? "";
    const getNumber = (role) => {
        const raw = getValue(role);
        if (!raw) {
            return null;
        }
        const value = Number(raw);
        return Number.isFinite(value) ? value : null;
    };

    const firstName = getValue("author-first-name");
    const middleName = getValue("author-middle-name");
    const lastName = getValue("author-last-name");
    const birthYear = getNumber("author-birth-year");

    const required = [
        { key: "First Name", value: firstName },
        { key: "Last Name", value: lastName },
        { key: "Birth Year", value: birthYear }
    ];

    if (!validateRequired(output, required)) {
        return { ok: false };
    }

    return {
        ok: true,
        value: {
            firstName,
            middleName: middleName || null,
            lastName,
            birthYear
        }
    };
}

function collectUserPayload(panel, isUpdate, output) {
    const suffix = isUpdate ? "-update" : "";
    const getValue = (role) => panel.querySelector(`[data-role="${role}${suffix}"]`)?.value.trim() ?? "";

    const username = getValue("user-username");
    const email = getValue("user-email");
    const dob = getValue("user-dob");
    const accountCreationDate = getValue("user-account-date");

    const required = [
        { key: "Username", value: username },
        { key: "Email", value: email },
        { key: "Date of Birth", value: dob },
        { key: "Account Creation Date", value: accountCreationDate }
    ];

    if (!validateRequired(output, required)) {
        return { ok: false };
    }

    return {
        ok: true,
        value: {
            username,
            email,
            dob,
            accountCreationDate
        }
    };
}

function collectActivityPayload(panel, isUpdate, output) {
    const suffix = isUpdate ? "-update" : "";
    const getValue = (role) => panel.querySelector(`[data-role="${role}${suffix}"]`)?.value.trim() ?? "";
    const getNumber = (role) => {
        const raw = getValue(role);
        if (raw === "") {
            return null;
        }
        const value = Number(raw);
        return Number.isFinite(value) ? value : null;
    };

    const userId = getNumber("activity-user-id");
    const bookId = getNumber("activity-book-id");
    const bookStatus = getValue("activity-status");
    const progressCompleted = getNumber("activity-progress");
    const startDate = getValue("activity-start-date");
    const endDate = getValue("activity-end-date");

    const required = [
        { key: "User ID", value: userId },
        { key: "Book ID", value: bookId },
        { key: "Status", value: bookStatus },
        { key: "Progress %", value: progressCompleted }
    ];

    if (!validateRequired(output, required)) {
        return { ok: false };
    }

    return {
        ok: true,
        value: {
            userId,
            bookId,
            bookStatus,
            progressCompleted,
            startDate: startDate || null,
            endDate: endDate || null
        }
    };
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
                message: "Request timed out after 25 seconds.",
                hint: "The API may be cold-starting. Retry in a few seconds or confirm the base URL."
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
