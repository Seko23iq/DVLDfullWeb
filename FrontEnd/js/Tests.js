
const appointmentsTableBody    = document.getElementById("appointmentsTableBody");
const appPersonInfo    = document.getElementById("appPersonInfo");
let PersonID  = 0;

// ── Helpers ──────────────────────────────────
function getIDFromURL() {
    return new URLSearchParams(window.location.search).get("LocalApplicationID");
}
function getTestTypeIDFromURL() {
    return new URLSearchParams(window.location.search).get("TestTypeID");
}
function formatDate(dateStr) {
    if (!dateStr) return "—";
    return new Date(dateStr).toLocaleDateString("en-GB", {
        day: "2-digit", month: "short", year: "numeric"
    });
}

const TestTypeID = getTestTypeIDFromURL();

// ── Load ─────────────────────────────────────
window.addEventListener("load", () => {

    const id = getIDFromURL();

    if (id) {
        loadApplicationInfo(id);
        LoadThePageInfo();
        loadAppointments(id);
    }
});

appPersonInfo.addEventListener("click", () => {
    window.location.href = `/Sections/People/personInfo.html?id=${PersonID}`;
})

const pageHeadingTitle = document.querySelector(".page-title");
const breadcrumbTestTitle = document.querySelector(".breadcrumb span");
const pageTitleBadgeTitle = document.querySelector(".page-title-badge");
const pageSubTitle = document.querySelector(".page-subtitle");
function LoadThePageInfo()
{
    if(TestTypeID == 1)
    {
        pageHeadingTitle.textContent = "Vision Test Appointment";
        breadcrumbTestTitle.textContent = "Vision Test Appointment";
        pageTitleBadgeTitle.textContent = "Test Type · Vision";
        pageSubTitle.textContent = "Schedule and manage vision test appointments for this driving license application";
    }
    else if(TestTypeID == 2)
    {
        pageHeadingTitle.textContent = "Writing Test Appointment";
        breadcrumbTestTitle.textContent = "Writing Test Appointment";
        pageTitleBadgeTitle.textContent = "Test Type · Writing";
        pageSubTitle.textContent = "Schedule and manage writing test appointments for this driving license application";
    }
    else if(TestTypeID == 3)
    {
        pageHeadingTitle.textContent = "Street Test Appointment";
        breadcrumbTestTitle.textContent = "Street Test Appointment";
        pageTitleBadgeTitle.textContent = "Test Type · Street";
        pageSubTitle.textContent = "Schedule and manage street test appointments for this driving license application";
    }
}
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
// ── Fetch Appointments ────────────────────────
async function loadAppointments(id) {
    try {
        const res = await fetch(`https://localhost:7223/api/TestAppointment/ByApplication?LocalDrivingLicenseApplicationID=${id}&TestTypeID=${TestTypeID}`);
        // if (!res.ok) throw new Error("Failed to fetch appointments");
        if (!res.ok) return;
        const data = await res.json();
        renderAppointments(data);
    } catch (err) {
        console.error(err);
    }
}
function renderAppointments(list) {
    const tbody = document.getElementById("appointmentsTableBody");
    document.getElementById("appointmentsCount").textContent = list.length;
    if (!list || list.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="4">
                <div class="table-empty">
                    <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.4">
                        <rect x="3" y="4" width="14" height="13" rx="2"/>
                        <line x1="3" y1="8" x2="17" y2="8"/>
                        <line x1="7" y1="2" x2="7" y2="6"/>
                        <line x1="13" y1="2" x2="13" y2="6"/>
                    </svg>
                    No appointments scheduled yet
                </div>
            </td></tr>`;
        return;
    }
    tbody.innerHTML = list.map(a => 
        `<tr>
    <td class="td-id">#${a.testAppointmentID}</td>
    <td class="td-date">${formatDate(a.appointmentDate)}</td>
    <td class="td-fees">${a.paidFees ? `$${a.paidFees}` : "—"}</td>

    <td>
        ${a.isLocked
            ? `<span class="lock-badge lock-yes">
                    <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="2">
                        <rect x="5" y="9" width="10" height="8" rx="1.5"/>
                        <path d="M7 9V6.5a3 3 0 0 1 6 0V9"/>
                    </svg>
                    Locked
               </span>`
            : `<span class="lock-badge lock-no">
                    <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="2">
                        <rect x="5" y="9" width="10" height="8" rx="1.5"/>
                        <path d="M7 9V6.5a3 3 0 0 1 4.5-.5"/>
                    </svg>
                    Open
               </span>`
        }
    </td>

    <td>
        <div class="td-options">

            <button 
                class="btn-table btn-edit ${a.isLocked ? "islocked" : ""}"
                onclick="editAppointment(${a.testAppointmentID})"
                ${a.isLocked ? "disabled" : ""}
            >
                <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.8">
                    <path d="M13.5 3.5l3 3L7 17H4v-3L13.5 3.5z"/>
                </svg>
                Edit
            </button>

            <button 
                class="btn-table btn-take ${a.isLocked ? "islocked" : ""}"
                onclick="takeTest(${a.testAppointmentID})"
                ${a.isLocked ? "disabled" : ""}
            >
                <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.8">
                    <circle cx="10" cy="10" r="7.5"/>
                    <path d="M7.5 10l2 2 3.5-3.5"/>
                </svg>
                Take Test
            </button>

        </div>
    </td>
</tr>`).join("");



}


// ── Add Appointment ───────────────────────────
async function addAppointment() {
    
    const LocalApplicationID = getIDFromURL();

    const payload = {
        localDrivingLicenseApplicationID: LocalApplicationID,
        testTypeID: TestTypeID,                                           
    };
        
    try {

        const res = await fetch(`https://localhost:7223/api/TestAppointment/HasActiveTestAppointment`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
        });        
        if (!res.ok) throw new Error("Failed to fetch appointments");

        
        const data = await res.json();

        if(data)
        {
            showToast("Person already have an active appointment for this test, you cannot add new appointment.", "error");
            return;
        }
        else 
        {
            window.location.href = `/Sections/Tests/scheduleTest.html?LocalApplicationID=${LocalApplicationID}&TestTypeID=${TestTypeID}`;
        }
    } catch (err) {
        console.error(err);
    }
    

}
// ── Dropdown toggle ───────────────────────────
function toggleDropdown(id) {
    const dropdown = document.getElementById(id);
    const isOpen = dropdown.classList.contains("open");
    document.querySelectorAll(".nav-dropdown.open").forEach(d => d.classList.remove("open"));
    if (!isOpen) dropdown.classList.add("open");
}

// ── Table click delegation ────────────────────
appointmentsTableBody.addEventListener("click", function (e) {

    // Info
    const editBtn = e.target.closest(".btn-edit");

    if (editBtn) {
        const testAppointmentID = editBtn
        .closest("tr")
        .cells[0]
        .textContent
        .trim()
        .replace("#", "");

        const id = getIDFromURL();

        window.location.href = `/Sections/Tests/editscheduleTest.html?LocalApplicationID=${id}&testAppointmentID=${testAppointmentID}&TestTypeID=${TestTypeID}`;
        return;
    }

    // Take Test
    const takeBtn = e.target.closest(".btn-take");
    
    if (takeBtn) {
        const testAppointmentID = takeBtn
        .closest("tr")
        .cells[0]
        .textContent
        .trim()
        .replace("#", "");

        window.location.href = `/Sections/Tests/takeTest.html?TestAppointmentID=${testAppointmentID}&TestTypeID=${TestTypeID}`; // 
        return;
    }
});

function editAppointment(id) {
    // your edit logic here

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



