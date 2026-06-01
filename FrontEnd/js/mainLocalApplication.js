// ═══════════════════════════════════════════════
//  Local Manage Application Table 
// ═══════════════════════════════════════════════


const ROWS_PER_PAGE = 6;

let currentPage = 1;
let totalPages  = 1;
let allPeople   = [];   

const filterBy     = document.getElementById("filterBy");
const filterInput  = document.getElementById("filterInput");
const statusSelect = document.getElementById("statusSelect");
const errorMsg     = document.getElementById("errorMsg");
const tableBody    = document.getElementById("LocalApplicationTableBody");
const pageInfo     = document.getElementById("pageInfo");
const totalCount   = document.getElementById("totalCount");
const pgNumbers    = document.getElementById("pgNumbers");

document.getElementById("btnFirst").addEventListener("click", () => goToPage(1));
document.getElementById("btnPrev").addEventListener("click",  () => goToPage(currentPage - 1));
document.getElementById("btnNext").addEventListener("click",  () => goToPage(currentPage + 1));
document.getElementById("btnLast").addEventListener("click",  () => goToPage(totalPages));

window.addEventListener("load", () =>
{
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");

    if(userNameElement)
    {
        if(storedName)
        {
            userNameElement.textContent = storedName;
        }
        else 
        {
            userNameElement.textContent = "Guest"; // This sholud return null;
        }
    }
    loadFilteredPeople();

   
    
});

// ── Filter: change type ───────────────────────
filterBy.addEventListener("change", function () {
    filterInput.value = "";
    filterInput.style.display  = "none";
    filterInput.style.border   = "1.5px solid var(--line)";
    statusSelect.style.display = "none";
    errorMsg.style.display     = "none";

    if (this.value === "Status") {
        statusSelect.style.display = "block";
        statusSelect.value = "";
    } else if (this.value !== "") {
        filterInput.style.display = "block";
        filterInput.focus();
    }

    currentPage = 1;
    loadFilteredPeople();
});

// ── Filter: input ─────────────────────────────
filterInput.addEventListener("input", function () {
    const value = this.value;
    const type  = filterBy.value;
    let isValid = true;
    let message = "";

    if (type === "PersonID") {
        isValid = /^[0-9]*$/.test(value);
        message = "Person ID must contain numbers only";
    } else if (type === "Name") {
        isValid = /^[a-zA-Z\s]*$/.test(value);
        message = "Name must contain letters only";
    } else if (type === "Email") {
        isValid = value === "" || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
        message = "Invalid email format";
    }

    if (!isValid && value !== "") {
        errorMsg.textContent   = message;
        errorMsg.style.display = "block";
        this.style.border      = "1.5px solid red";
    } else {
        errorMsg.style.display = "none";
        this.style.border      = "1.5px solid var(--line)";
    }

    clearTimeout(filterInput._timer);
    filterInput._timer = setTimeout(() => {
        currentPage = 1;
        loadFilteredPeople();
    }, 400);
});

statusSelect.addEventListener("change", () => {
    currentPage = 1;
    loadFilteredPeople();
});

// ── Fetch data ────────────────────────────────
async function loadFilteredPeople() {
    try {
        const filterTypeName = filterBy.value;
        let filterType;

        switch (filterTypeName) {
            case "L.D.L AppID": filterType = 0; break;
            // case "NationalNo": filterType = 1; break;
            case "Full Name": filterType = 1; break;
            case "Status": filterType = 2; break;
            default:  break;
        }
        
        let value = filterType === 2
            ? statusSelect.value
            : filterInput.value.trim();

        let url = "https://localhost:7223/api/LocalDrivingApplication/All";
        if (filterType !== undefined && value !== undefined && value !== "") {
            url = `https://localhost:7223/api/LocalDrivingApplication/Filter?filterType=${encodeURIComponent(filterType)}&value=${encodeURIComponent(value)}`;
        }

        const response = await fetch(url);
        if (!response.ok) throw new Error("Failed to fetch");

        allLocalDrivingApplications = await response.json();
        totalPages = Math.max(1, Math.ceil(allLocalDrivingApplications.length / ROWS_PER_PAGE));

        renderPage(currentPage);
        renderPagination();

    } catch (err) {
        console.error(err);
        tableBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#a32d2d;padding:24px;">
                Failed to load data.
            </td></tr>`;
    }
}

// ── Render current page rows ──────────────────
function renderPage(page) {
    currentPage = Math.min(Math.max(1, page), totalPages);

    const start = (currentPage - 1) * ROWS_PER_PAGE;
    const slice = allLocalDrivingApplications.slice(start, start + ROWS_PER_PAGE);

    tableBody.innerHTML = "";

    if (slice.length === 0) {
        tableBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:var(--muted);padding:32px;">
                No records found.
            </td></tr>`;
        return;
    }
    slice.forEach(LocalApplication => {
        let LocalApplicationClass = "badge-new"; 
        if(LocalApplication.status === "New")
        {
            LocalApplicationClass = "badge-new";
        } else if (LocalApplication.status === "Cancelled")
        {
            LocalApplicationClass = "badge-canceled";
        } else if (LocalApplication.status === "Completed")
        {
            LocalApplicationClass = "badge-complete";
        }
        // <td>${LocalApplication.nationalNo || "—"}</td>

        tableBody.innerHTML += `
            <tr>
                <td>${LocalApplication.l_D_L_AppID}</td>
                <td>${LocalApplication.drivingClass}</td>
                <td>${LocalApplication.fullName}</td>
                <td><span class="badge ${LocalApplicationClass}">${LocalApplication.status}</span></td>
                <td>${LocalApplication.passedTests}</td>
                <td>${makeOptionsBtns(LocalApplication.status)}</td>
                </tr>`;
            });
            
            // <td>${makeOptionsBtns()}</td>
    // تحديث info
    const from = start + 1;
    const to   = Math.min(start + ROWS_PER_PAGE, allLocalDrivingApplications.length);
    pageInfo.textContent  = `Page ${currentPage} of ${totalPages}`;
    totalCount.textContent = `Showing ${from}–${to} of ${allLocalDrivingApplications.length} people`;
}

// ── Pagination controls ───────────────────────
function goToPage(page) {
    if (page < 1 || page > totalPages) return;
    renderPage(page);
    renderPagination();
}

function renderPagination() {
    pgNumbers.innerHTML = "";

    document.getElementById("btnFirst").disabled = currentPage === 1;
    document.getElementById("btnPrev").disabled  = currentPage === 1;
    document.getElementById("btnNext").disabled  = currentPage === totalPages;
    document.getElementById("btnLast").disabled  = currentPage === totalPages;

    // نافذة الأرقام: تظهر 5 أرقام max حول الصفحة الحالية
    const range  = 2;
    const start  = Math.max(1, currentPage - range);
    const end    = Math.min(totalPages, currentPage + range);

    if (start > 1) {
        pgNumbers.appendChild(makePageBtn(1));
        if (start > 2) pgNumbers.appendChild(makeEllipsis());
    }

    for (let i = start; i <= end; i++) {
        pgNumbers.appendChild(makePageBtn(i));
    }

    if (end < totalPages) {
        if (end < totalPages - 1) pgNumbers.appendChild(makeEllipsis());
        pgNumbers.appendChild(makePageBtn(totalPages));
    }
}

function makePageBtn(n) {
    const btn = document.createElement("button");
    btn.className = "pg-btn pg-num" + (n === currentPage ? " active" : "");
    btn.textContent = n;
    btn.onclick = () => goToPage(n);
    return btn;
}

function makeEllipsis() {
    const span = document.createElement("span");
    span.className = "pg-ellipsis";
    span.textContent = "…";
    return span;
}

// ── Table click delegation ────────────────────
tableBody.addEventListener("click", function (e) {

    // Info
    const infoBtn = e.target.closest(".btn-info");
    if (infoBtn) {
        const LDLA_ID = infoBtn.closest("tr").cells[0].textContent.trim();
        const passedTests = infoBtn.closest("tr").cells[4].textContent.trim();
        window.location.href = `/Sections/Applications/LocalApplication/localApplicationInfo.html?LocalApplicationID=${LDLA_ID}&TestTypeID=${Number(passedTests) + 1}`;
        return;
    }

    // Cancel
    const cancelBtn = e.target.closest(".btn-cancel");
    if (cancelBtn) {
        const LocalApplicationID = cancelBtn.closest("tr").cells[0].textContent.trim();
        CancelApplication(LocalApplicationID);
        return;
    }

    // Delete
    const deleteBtn = e.target.closest(".btn-delete");
    if (deleteBtn) {
        const LocalApplicationID = deleteBtn.closest("tr").cells[0].textContent.trim();
        
        console.log(typeof LocalApplicationID);
        if (confirm(`Are you sure you want to delete This application with ID: ${LocalApplicationID}?`)) {
            DeleteApplication(LocalApplicationID);
            return;
        }
    }

      // Edit
    const processBtn = e.target.closest(".btn-process");
    if (processBtn) {
        const LDAppID = processBtn.closest("tr").cells[0].textContent.trim();
        console.log("Processing the Application :", LDAppID);
        window.location.href = `/Sections/Applications/LocalApplication/ProcessingLA.html?id=${LDAppID}`; // 
        return;
    }
});


function makeOptionsBtns(status) {
    const isDisabled = status === "Completed" || status === "Cancelled";
    return `
        <div class="options">
            <button class="btn-info">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <circle cx="8" cy="8" r="6"/>
                    <line x1="8" y1="7" x2="8" y2="11"/>
                    <circle cx="8" cy="5" r=".6" fill="currentColor" stroke="none"/>
                </svg>Info
            </button>
            <button class="btn-cancel" ${isDisabled ? "disabled" : ""}>
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <circle cx="8" cy="8" r="6"/>
                    <line x1="5.5" y1="5.5" x2="10.5" y2="10.5"/>
                    <line x1="10.5" y1="5.5" x2="5.5" y2="10.5"/>
                </svg>Cancel
            </button>
            <button class="btn-delete"  ${isDisabled ? "disabled" : ""}>
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <polyline points="3,5 13,5"/>
                    <path d="M6 5V3h4v2"/>
                    <path d="M5 5l.7 8h4.6l.7-8"/>
                </svg>Delete
            </button>
            <button class="btn-process">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <circle cx="8" cy="8" r="6"/>
                    <path d="M6.5 5.5l4 2.5-4 2.5V5.5z" fill="currentColor" stroke="none"/>
                </svg>Processing
            </button>
        </div>`;
}
// ── Dropdown toggle ────────────────────────────────────────────
function toggleDropdown(id) {
    const dropdown = document.getElementById(id);
    const isOpen = dropdown.classList.contains("open");
    // Close all dropdowns first
    document.querySelectorAll(".nav-dropdown.open").forEach(d => d.classList.remove("open"));
    // Toggle the clicked one
    if (!isOpen) {
        dropdown.classList.add("open");
    }
}


async function CancelApplication(LocalApplicationID)
{
    try
    {
        const response = await fetch(`https://localhost:7223/api/LocalDrivingApplication/${LocalApplicationID}/cancel`,
            {
                method: "PUT",
                headers: { "Content-Type": "application/json" }
            }
        );

        if(!response.ok)
        {
            throw new Error(response.body);
            return;
        }

        const data = await response.json();

        
        showToast(`Local Application with ID = ${LocalApplicationID} was canceled successfully.`, "success")

        setTimeout(() => {
            window.location.href = `/Sections/Applications/LocalApplication/mainLocalApplication.html`;
        }, 2000);

    }
    catch(error)
    {
        console.error("Error : " + error);
    }
}

async function DeleteApplication(LocalApplicationID)
{
    try
    {
        const response = await fetch(`https://localhost:7223/api/LocalDrivingApplication/${LocalApplicationID}/delete`,
            {
                method: "DELETE"
            }
        );

        // فشل الطلب
        if (!response.ok)
        {
            const errorText = await response.text();
            throw new Error(errorText);
        }

        // قراءة البيانات إذا موجودة
        let data = null;

        const contentType = response.headers.get("content-type");

        if (contentType && contentType.includes("application/json"))
        {
            data = await response.json();
        }

        console.log(data);

        showToast(
            `Local Application with ID = ${LocalApplicationID} was deleted successfully.`,
            "success"
        );

        setTimeout(() => {

            window.location.href =
            `/Sections/Applications/LocalApplication/mainLocalApplication.html`;

        }, 2000);

    }
    catch(error)
    {
        console.error("Error:", error);

        showToast(error.message, "error");
    }
}

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