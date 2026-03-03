const panels = document.querySelectorAll(".panel");

for (const panel of panels) {
    const entity = panel.dataset.entity;
    const output = panel.querySelector('[data-role="output"]');

    panel.querySelector('[data-action="all"]').addEventListener("click", async () => {
        await handleGetAll(entity, output);
    });

    panel.querySelector('[data-action="single"]').addEventListener("click", async () => {
        const idText = panel.querySelector('[data-role="id-input"]').value.trim();
        await handleGetById(entity, idText, output);
    });

    panel.querySelector('[data-action="subset"]').addEventListener("click", async () => {
        const field = panel.querySelector('[data-role="filter-field"]').value.trim();
        const value = panel.querySelector('[data-role="filter-value"]').value.trim();
        await handleGetSubset(entity, field, value, output);
    });

    panel.querySelector('[data-action="create"]').addEventListener("click", async () => {
        const jsonText = panel.querySelector('[data-role="create-json"]').value.trim();
        await handleCreate(entity, jsonText, output);
    });

    panel.querySelector('[data-action="update"]').addEventListener("click", async () => {
        const idText = panel.querySelector('[data-role="update-id"]').value.trim();
        const jsonText = panel.querySelector('[data-role="update-json"]').value.trim();
        await handleUpdate(entity, idText, jsonText, output);
    });

    panel.querySelector('[data-action="delete"]').addEventListener("click", async () => {
        const idText = panel.querySelector('[data-role="delete-id"]').value.trim();
        await handleDelete(entity, idText, output);
    });
}

function getApiBaseUrl() {
    return document.getElementById("apiBaseUrl").value.trim().replace(/\/$/, "");
}

async function handleGetAll(entity, output) {
    try {
        setOutput(output, `Loading all ${entity}...`);
        const response = await fetch(`${getApiBaseUrl()}/api/${entity}`);
        await writeResponse(output, response);
    } catch (error) {
        setOutput(output, `Request failed: ${error}`);
    }
}

async function handleGetById(entity, idText, output) {
    if (!idText) {
        setOutput(output, "Provide an id value.");
        return;
    }

    try {
        setOutput(output, `Loading ${entity} id=${idText}...`);
        const response = await fetch(`${getApiBaseUrl()}/api/${entity}/${encodeURIComponent(idText)}`);
        await writeResponse(output, response);
    } catch (error) {
        setOutput(output, `Request failed: ${error}`);
    }
}

async function handleGetSubset(entity, field, value, output) {
    if (!field || !value) {
        setOutput(output, "Provide filter field and value.");
        return;
    }

    try {
        setOutput(output, `Loading subset from ${entity} where ${field} includes "${value}"...`);
        const response = await fetch(`${getApiBaseUrl()}/api/${entity}`);
        if (!response.ok) {
            await writeResponse(output, response);
            return;
        }

        const rows = await response.json();
        const subset = rows.filter((row) => {
            const fieldValue = getCaseInsensitivePropertyValue(row, field);
            if (fieldValue === undefined || fieldValue === null) {
                return false;
            }

            return String(fieldValue).toLowerCase().includes(value.toLowerCase());
        });

        setOutput(output, JSON.stringify(subset, null, 2));
    } catch (error) {
        setOutput(output, `Request failed: ${error}`);
    }
}

async function handleCreate(entity, jsonText, output) {
    const payload = parseJsonPayload(jsonText, output);
    if (!payload.ok) {
        return;
    }

    try {
        setOutput(output, `Creating ${entity}...`);
        const response = await fetch(`${getApiBaseUrl()}/api/${entity}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload.value)
        });
        await writeResponse(output, response);
    } catch (error) {
        setOutput(output, `Request failed: ${error}`);
    }
}

async function handleUpdate(entity, idText, jsonText, output) {
    if (!idText) {
        setOutput(output, "Provide an id for update.");
        return;
    }

    const payload = parseJsonPayload(jsonText, output);
    if (!payload.ok) {
        return;
    }

    try {
        setOutput(output, `Updating ${entity} id=${idText}...`);
        const response = await fetch(`${getApiBaseUrl()}/api/${entity}/${encodeURIComponent(idText)}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload.value)
        });
        await writeResponse(output, response);
    } catch (error) {
        setOutput(output, `Request failed: ${error}`);
    }
}

async function handleDelete(entity, idText, output) {
    if (!idText) {
        setOutput(output, "Provide an id for delete.");
        return;
    }

    try {
        setOutput(output, `Deleting ${entity} id=${idText}...`);
        const response = await fetch(`${getApiBaseUrl()}/api/${entity}/${encodeURIComponent(idText)}`, {
            method: "DELETE"
        });
        await writeResponse(output, response);
    } catch (error) {
        setOutput(output, `Request failed: ${error}`);
    }
}

async function writeResponse(output, response) {
    const text = await response.text();
    const parsed = tryParseJson(text);
    const body = parsed.ok ? JSON.stringify(parsed.value, null, 2) : text;

    const payload = {
        status: response.status,
        ok: response.ok,
        body
    };

    setOutput(output, JSON.stringify(payload, null, 2));
}

function tryParseJson(text) {
    try {
        return { ok: true, value: JSON.parse(text) };
    } catch {
        return { ok: false };
    }
}

function setOutput(output, value) {
    output.textContent = value;
}

function getCaseInsensitivePropertyValue(obj, key) {
    const match = Object.keys(obj).find((k) => k.toLowerCase() === key.toLowerCase());
    return match ? obj[match] : undefined;
}

function parseJsonPayload(jsonText, output) {
    if (!jsonText) {
        setOutput(output, "JSON payload is required.");
        return { ok: false };
    }

    try {
        return { ok: true, value: JSON.parse(jsonText) };
    } catch (error) {
        setOutput(output, `Invalid JSON payload: ${error}`);
        return { ok: false };
    }
}
