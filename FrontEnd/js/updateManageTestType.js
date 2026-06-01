const BASE_URL = "https://localhost:7223/api/ApplicationTestTypes";

// ── DOM refs ──────────────────────────────────
const loadingState = document.getElementById("loadingState");
const errorState   = document.getElementById("errorState");
const errorMsg     = document.getElementById("errorMsg");
const formCard     = document.getElementById("formCard");

const fieldID      = document.getElementById("fieldID");
const fieldTitle   = document.getElementById("fieldTitle");
const fieldDescription   = document.getElementById("fieldDescription");
const fieldFees    = document.getElementById("fieldFees");

const titleError   = document.getElementById("titleError");
const feesError    = document.getElementById("feesError");

const btnSave      = document.getElementById("btnSave");
const toast        = document.getElementById("toast");

// ── Read ID from URL ──────────────────────────
const params = new URLSearchParams(window.location.search);
const applicationTestTypeID = parseInt(params.get("id"));

// ── Load on start ─────────────────────────────
window.addEventListener("load", () => {
    // Set username in sidebar
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");
    if (userNameElement) {
        userNameElement.textContent = storedName || "Guest";
    }

    if (!applicationTestTypeID || isNaN(applicationTestTypeID)) {
        showError("Invalid or missing Application Type ID.");
        return;
    }

    loadApplicationType();
});

btnSave.addEventListener("click", () => {
    submitUpdate();
})

// ── Fetch existing data ────────────────────────
async function loadApplicationType() {
    showLoading();
    try {
        const response = await fetch(`${BASE_URL}/ApplictionTestTypeInfo/${applicationTestTypeID}`);


        if (!response.ok) throw new Error(`Server returned ${response.status}`);
	
        const data = await response.json();

        // Populate fields
        fieldID.value    = data.id    ?? applicationTypeID;
        fieldTitle.value = data.title ?? "";
        fieldDescription.value = data.description ?? "";
        fieldFees.value  = data.fees      ?? 0;
        showForm();


    } catch (err) {
        console.error(err);
        showError("Failed to load Application Type data. Please try again.");
    }
}

// ── Validate ───────────────────────────────────
function validate() {
    let valid = true;

    // Title
    const title = fieldTitle.value.trim();
    if (!title) {
        titleError.textContent = "Title is required.";
        titleError.classList.remove("hidden");
        fieldTitle.classList.add("invalid");
        valid = false;
    } else {
        titleError.classList.add("hidden");
        fieldTitle.classList.remove("invalid");
    }

    // Fees
    const fees = parseFloat(fieldFees.value);
    if (fieldFees.value === "" || isNaN(fees) || fees < 0) {
        feesError.textContent = "Fees must be 0 or greater.";
        feesError.classList.remove("hidden");
        fieldFees.classList.add("invalid");
        valid = false;
    } else {
        feesError.classList.add("hidden");
        fieldFees.classList.remove("invalid");
    }

    return valid;
}

// ── Submit update ──────────────────────────────
async function submitUpdate() {
    if (!validate()) return;

    btnSave.disabled = true;
    btnSave.textContent = "Saving…";
	
    const payload = {
        id:             applicationTestTypeID,
        title:          fieldTitle.value.trim(),
        description:    fieldDescription.value.trim(),
        fees:           parseFloat(fieldFees.value)
    };

    try {
        const response = await fetch(`${BASE_URL}/Update/${applicationTestTypeID}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error(`Server returned ${response.status}`);

        showToast("Application Type updated successfully!", "success");

        // Redirect back after short delay
        setTimeout(() => {
            window.location.href = "/Sections/Applications/ManageTestTypes/ManageTestTypes.html";
        }, 1500);

    } catch (err) {
        console.error(err);
        showToast("Failed to save changes. Please try again.", "error");
    } finally {
        btnSave.disabled = false;
        btnSave.innerHTML = `
            <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.8">
                <path d="M13 2H5L2 5v9h12V2zM10 2v4H5V2M8 9a2 2 0 1 0 0 4 2 2 0 0 0 0-4z"/>
            </svg>
            Save Changes`;
    }
}

// ── Toast helper ───────────────────────────────
function showToast(message, type = "success") {
    toast.textContent = message;
    toast.className = `toast ${type}`;
    toast.classList.remove("hidden");

    setTimeout(() => {
        toast.classList.add("hidden");
    }, 4000);
}

// ── UI State helpers ───────────────────────────
function showLoading() {
    loadingState.classList.remove("hidden");
    errorState.classList.add("hidden");
    formCard.classList.add("hidden");
}

function showError(message) {
    errorMsg.textContent = message;
    loadingState.classList.add("hidden");
    errorState.classList.remove("hidden");
    formCard.classList.add("hidden");
}

function showForm() {
    loadingState.classList.add("hidden");
    errorState.classList.add("hidden");
    formCard.classList.remove("hidden");
}