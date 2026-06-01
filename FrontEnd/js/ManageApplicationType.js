import { showToast } from "./Global.js";


const ROWS_PER_PAGE = 7;

let currentPage = 1;
let totalPages  = 1;
let allApplicationTypes    = [];   // كل البيانات المجلوبة

// ── DOM refs ─────────────────────────────────
const isActiveSelect = document.getElementById("isActiveSelect");
const errorMsg       = document.getElementById("errorMsg");
const tableBody      = document.getElementById("ManageApplicationTypeTableBody");
const pageInfo       = document.getElementById("pageInfo");
const totalCount     = document.getElementById("totalCount");
const pgNumbers      = document.getElementById("pgNumbers");

let isActive;
window.addEventListener("load", () =>
{
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");
    isActive = localStorage.getItem("isActive");
    if (userNameElement) {
        userNameElement.textContent = storedName || "Guest";
    }

    loadManageApplicationTypes();
});

// ── Fetch data ────────────────────────────────
async function loadManageApplicationTypes() {
    try {
        let url = "https://localhost:7223/api/ApplicationTypes/All";

        const response = await fetch(url);
        if (!response.ok) throw new Error("Failed to fetch");

        allApplicationTypes   = await response.json();

        totalPages = Math.max(1, Math.ceil(allApplicationTypes.length / ROWS_PER_PAGE));

        renderPage(currentPage);
        renderPagination();

    } catch (err) {
        console.error(err);
        tableBody.innerHTML = `
            <tr><td colspan="6" style="text-align:center;color:#a32d2d;padding:24px;">
                Failed to load data.
            </td></tr>`;
    }
}

// ── Render current page rows ──────────────────
function renderPage(page) {
    currentPage = Math.min(Math.max(1, page), totalPages);

    const start = (currentPage - 1) * ROWS_PER_PAGE;
    const slice = allApplicationTypes.slice(start, start + ROWS_PER_PAGE);

    tableBody.innerHTML = "";

    if (slice.length === 0) {
        tableBody.innerHTML = `
            <tr><td colspan="6" style="text-align:center;color:var(--muted);padding:32px;">
                No records found.
            </td></tr>`;
        return;
    }

    slice.forEach(ApplicationType => {
        const id    = ApplicationType.id ?? "—";
        const title    = ApplicationType.title ?? "—";
        const fees    = ApplicationType.fees ?? "—";

        tableBody.innerHTML += `
            <tr>
                <td>${id}</td>
                <td>${title}</td>
                <td>${fees}</td>
                <td>${makeOptionsBtns()}</td>
            </tr>`;
    });

    // تحديث info
    const from = start + 1;
    const to   = Math.min(start + ROWS_PER_PAGE, allApplicationTypes.length);
    pageInfo.textContent   = `Page ${currentPage} of ${totalPages}`;
    totalCount.textContent = `Showing ${from}–${to} of ${allApplicationTypes.length} Application Types`;
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

    const range = 2;
    const start = Math.max(1, currentPage - range);
    const end   = Math.min(totalPages, currentPage + range);

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

// ── Options buttons ───────────────────────────
function makeOptionsBtns() {
    return `
        <div class="options">
            <button class="btn-edit">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <path d="M11 2.5l2.5 2.5-7 7H4v-2.5l7-7z"/>
                </svg>Edit
            </button>
        </div>`;
}

// ── Table click delegation ────────────────────
tableBody.addEventListener("click", function (e) {
    // Edit
    const editBtn = e.target.closest(".btn-edit");

    if(isActive === "false")
    {
        showToast("you are not active user in system, you can't edit any data", "error");
        return;
    }
    if (editBtn) {
        const ManageApplicationTypeID = editBtn.closest("tr").cells[0].textContent.trim();
        window.location.href = `/Sections/Applications/ManageApplicationTypes/updateManageApplicationType.html?id=${ManageApplicationTypeID}`;
        return;
    }

});