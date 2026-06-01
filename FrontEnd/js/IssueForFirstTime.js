
const appointmentsTableBody    = document.getElementById("appointmentsTableBody");

// ── Helpers ──────────────────────────────────
function getIDFromURL() {
    return new URLSearchParams(window.location.search).get("LocalApplicationID");
}

function formatDate(dateStr) {
    if (!dateStr) return "—";
    return new Date(dateStr).toLocaleDateString("en-GB", {
        day: "2-digit", month: "short", year: "numeric"
    });
}

// ── Load ─────────────────────────────────────
window.addEventListener("load", () => {
  
    const id = getIDFromURL();
    if (id) {
        loadApplicationInfo(id);
    }
});


// ── Fetch Application Info ────────────────────
async function loadApplicationInfo(id) {
    try {
        const res = await fetch(`https://localhost:7223/api/LocalDrivingApplication/ApplicationBasicInfo?LocalDrivingLicenseApplicationID=${id}`);
        if (!res.ok) throw new Error("Failed to fetch application info");
        const data = await res.json();
        renderApplicationInfo(data);
    } catch (err) {
        console.error(err);
    }
}
let PersonID = 0;
let ApplicationID = 0;
let licenseClassID = 1;
function renderApplicationInfo(data) {
    // DL App Info

    console.log(data);
    PersonID = data.personID;
    ApplicationID = data.applicationID;
    licenseClassID = data.licenseClassID;

    document.getElementById("dlAppID").textContent      = data.localDrivingLicenseApplicationID     || "—";
    document.getElementById("dlAppliedFor").textContent = data.className     || "—";
    const passed = data.passedTests  ?? 0;
    const total  = data.totalTests   ?? 3;
    const pct    = total > 0 ? Math.round((passed / total) * 100) : 0;
    document.getElementById("passedTestsLabel").textContent = `${passed} / ${total} tests passed`;
    document.getElementById("passedTestsPct").textContent   = `${pct}%`;
    document.getElementById("progressFill").style.width     = `${pct}%`;
    // Basic Info
    document.getElementById("appID").textContent          = data.applicationID  || "—";
    document.getElementById("appDate").textContent        = formatDate(data.applicationDate);
    document.getElementById("appFees").textContent        = data.fees ? `$${data.fees}` : "—";
    document.getElementById("appCreatedBy").textContent = data.createdBy ?  `$${data.createdBy}` : "—";
    document.getElementById("appType").textContent        = data.applicationType   || "—";
    document.getElementById("appApplicant").textContent   = data.applicant          || "—";
    document.getElementById("appStatusDate").textContent  = formatDate(data.lastStatusDate);
    // Status badge
    const statusEl = document.getElementById("appStatus");

    const statusMap = {
        "New": "badge-new",
        "Active": "badge-active",
        "Completed": "badge-complete",
        "Canceled": "badge-canceled",
        "Pending": "badge-pending"
    };

    const statusText = {
        1: "New",
        2: "Canceled",
        3: "Completed"
    };

    const status = statusText[data.status] || "Unknown";

    const cls = statusMap[status] || "badge-new";

    statusEl.innerHTML = `
        <span class="badge ${cls}">
            ${status}
        </span>
    `;
}
// ── Dropdown toggle ───────────────────────────
function toggleDropdown(id) {
    const dropdown = document.getElementById(id);
    const isOpen = dropdown.classList.contains("open");
    document.querySelectorAll(".nav-dropdown.open").forEach(d => d.classList.remove("open"));
    if (!isOpen) dropdown.classList.add("open");
}

const processApplication = document.getElementById("processApplication");
processApplication.addEventListener("click", () => {
    const id = getIDFromURL();
    window.location.href = `/Sections/Applications/LocalApplication/ProcessingLA.html?id=${id}`;
})

function showToast(message, type = "success") {
    const toast = document.getElementById('toast');

    toast.textContent = message;

    toast.classList.remove('success', 'error');

    if (type.toLowerCase() === "error") {
        toast.classList.add('error');
    } else {
        toast.classList.add('success');
    }

    toast.classList.add('show');

    setTimeout(() => {
        toast.classList.remove('show');
    }, 3500);
}



// ── Issue & Cancel Handlers ───────────────────

function handleIssue() {
    const id = getIDFromURL();
    if (!id) {
        showToast("Application ID not found.", "error");
        return;
    }

    const notes = document.getElementById("issueNotes").value.trim();

    issueDriverLicense(id, notes);
}

const btnIssue = document.getElementById("btnIssue");
const btnCancel = document.getElementById("btnCancel");
btnIssue.addEventListener("click", handleIssue);
btnCancel.addEventListener("click", handleCancel);

async function issueDriverLicense(LocalApplicationID, notes) {
    btnIssue.disabled = true;
    btnIssue.innerHTML = `
        <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="2" style="width:16px;height:16px;animation:spin 0.8s linear infinite">
            <circle cx="10" cy="10" r="7" stroke-opacity="0.3"/>
            <path d="M10 3a7 7 0 0 1 7 7"/>
        </svg>
        Issuing…
    `;

    try {
        const res = await fetch(`https://localhost:7223/api/LocalDrivingApplication/IssueDrivingLicenseFirstTime`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                personID: PersonID,
                applicationID: ApplicationID,
                licenseClass: licenseClassID,
                notes: notes
            })
        });

        if (!res.ok) {
            const errText = await res.text();
            throw new Error(errText || "Failed to issue license");
        }
        
        const data = await res.json();
        showToast(`License Issued Successfully with License ID = ${data.newID}`, "success");

        // Disable cancel after successful issue
        const btnCancel = document.getElementById("btnCancel");
        if (btnCancel) btnCancel.disabled = true;


        // Reload info to reflect updated status
        setTimeout(() => window.location.href = `/Sections/Applications/LocalApplication/ProcessingLA.html?id=${LocalApplicationID}`, 2500);

    } catch (err) {
        console.error(err);
        showToast(err.message || "Something went wrong.", "error");
    } finally {
        btnIssue.disabled = false;
        btnIssue.innerHTML = `
            <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="2" style="width:16px;height:16px">
                <polyline points="3 10 8 15 17 5"/>
            </svg>
            Issue License
        `;
    }
}

function handleCancel() {
    const id = getIDFromURL();
    if (confirm("Are you sure you want to cancel this operation?")) {
        window.location.href = `/Sections/Applications/LocalApplication/mainLocalApplication.html`;
    }
}
// ── Spin animation (for loading button) ───────

const spinStyle = document.createElement("style");
spinStyle.textContent = `
    @keyframes spin {
        from { transform: rotate(0deg); }
        to   { transform: rotate(360deg); }
    }
`;
document.head.appendChild(spinStyle);


// ── Sub-dropdown toggle ───────────────────────

function toggleSubDropdown(id, event) {
    event.stopPropagation();
    const el = document.getElementById(id);
    el.classList.toggle("open");
}