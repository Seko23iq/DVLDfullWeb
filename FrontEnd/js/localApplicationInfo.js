import {  formatDate} from "./Global.js";

const appPersonInfo = document.getElementById("appPersonInfo");
let PersonID  = 0;

// ── Helpers ──────────────────────────────────
function getIDFromURL() {
    return new URLSearchParams(window.location.search).get("LocalApplicationID");
}
function getTestTypeIDFromURL() {
    return new URLSearchParams(window.location.search).get("TestTypeID");
}

const TestTypeID = getTestTypeIDFromURL();

window.addEventListener("load", () => {
    const id = getIDFromURL();

    if (id) {
        loadApplicationInfo(id);
    }
});

appPersonInfo.addEventListener("click", () => {
    window.location.href = `/Sections/People/personInfo.html?id=${PersonID}`;
})

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
function renderApplicationInfo(data) {
    // DL App Info
    const UserName = localStorage.getItem("UserName");

    PersonID=data.personID;

    console.log(data);
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
    document.getElementById("appCreatedBy").textContent   = UserName || "—";
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

