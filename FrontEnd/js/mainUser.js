import {showToast} from "./Global.js";

const ROWS_PER_PAGE = 6;

let currentPage = 1;
let totalPages  = 1;
let allUsers    = [];   

const filterBy       = document.getElementById("filterBy");
const filterInput    = document.getElementById("filterInput");
const isActiveSelect = document.getElementById("isActiveSelect");
const errorMsg       = document.getElementById("errorMsg");
const tableBody      = document.getElementById("usersTableBody");
const pageInfo       = document.getElementById("pageInfo");
const totalCount     = document.getElementById("totalCount");
const pgNumbers      = document.getElementById("pgNumbers");


document.getElementById("btnFirst").addEventListener("click", () => goToPage(1));
document.getElementById("btnPrev").addEventListener("click",  () => goToPage(currentPage - 1));
document.getElementById("btnNext").addEventListener("click",  () => goToPage(currentPage + 1));
document.getElementById("btnLast").addEventListener("click",  () => goToPage(totalPages));


let isActive ;
window.addEventListener("load", () =>
{
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");
    isActive = localStorage.getItem("isActive");

    if (userNameElement) {
        userNameElement.textContent = storedName || "Guest";
    }

    loadFilteredUsers();
});

// ── Filter: change type ───────────────────────
filterBy.addEventListener("change", function () {
    filterInput.value = "";
    filterInput.style.display    = "none";
    filterInput.style.border     = "1.5px solid var(--line)";
    isActiveSelect.style.display = "none";
    errorMsg.style.display       = "none";
    
    if (this.value === "IsActive") {
        isActiveSelect.style.display = "block";
        isActiveSelect.value = "";
    } else if (this.value !== "") {
        filterInput.style.display = "block";
        filterInput.focus();
    }

    currentPage = 1;
    loadFilteredUsers();
});

// ── Filter: input ─────────────────────────────
filterInput.addEventListener("input", function () {
    const value = this.value;
    const type  = filterBy.value;
    let isValid = true;
    let message = "";

    if (type === "UserID" || type === "PersonID") {
        isValid = /^[0-9]*$/.test(value);
        message = (type === "UserID" ? "User" : "Person") + " ID must contain numbers only";
    } else if (type === "FullName") {
        isValid = /^[a-zA-Z\s]*$/.test(value);
        message = "Full Name must contain letters only";
    } else if (type === "UserName") {
        isValid = /^[a-zA-Z0-9._\-]*$/.test(value);
        message = "Username can only contain letters, numbers, dots, underscores, or hyphens";
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
        loadFilteredUsers();
    }, 400);
});

isActiveSelect.addEventListener("change", () => {
    currentPage = 1;
    loadFilteredUsers();
});

// ── Fetch data ────────────────────────────────
async function loadFilteredUsers() {
    try {
        const filterType = filterBy.value;
        let value = filterType === "IsActive"
            ? isActiveSelect.value
            : filterInput.value.trim();

        let url = "https://localhost:7223/api/Users/All";
        if (filterType && value !== "") {
            if(filterType === "IsActive" && value === "false")
            {
                value = 0;
            }
            else  if(filterType === "IsActive" && value === "true")
            {
                value = 1;
            }

            url = `https://localhost:7223/api/Users/Filter?${encodeURIComponent(filterType)}=${encodeURIComponent(value)}`;
        }

        const response = await fetch(url);
        if (!response.ok) throw new Error("Failed to fetch");

        allUsers   = await response.json();
        totalPages = Math.max(1, Math.ceil(allUsers.length / ROWS_PER_PAGE));

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
    const slice = allUsers.slice(start, start + ROWS_PER_PAGE);

    tableBody.innerHTML = "";

    if (slice.length === 0) {
        tableBody.innerHTML = `
            <tr><td colspan="6" style="text-align:center;color:var(--muted);padding:32px;">
                No records found.
            </td></tr>`;
        return;
    }

    slice.forEach(user => {
        const fullName    = [user.firstName, user.secondName,user.thirdName,  user.lastName]
                            .filter(Boolean).join(" ") || user.fullName || "—";

        const isActive    = user.isActive === true || user.isActive === 1;
        const badgeClass  = isActive ? "badge-active"   : "badge-inactive";
        const badgeLabel  = isActive ? "Active"          : "Inactive";

        tableBody.innerHTML += `
            <tr>
                <td>${user.userID}</td>
                <td>${user.personID}</td>
                <td>${fullName}</td>
                <td class="username-cell">${user.userName || "—"}</td>
                <td>
                    <span class="badge ${badgeClass}">
                        <span class="badge-dot"></span>
                        ${badgeLabel}
                    </span>
                </td>
                <td>${makeOptionsBtns()}</td>
            </tr>`;
    });

    // تحديث info
    const from = start + 1;
    const to   = Math.min(start + ROWS_PER_PAGE, allUsers.length);
    pageInfo.textContent   = `Page ${currentPage} of ${totalPages}`;
    totalCount.textContent = `Showing ${from}–${to} of ${allUsers.length} users`;
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
            <button class="btn-info">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <circle cx="8" cy="8" r="6"/>
                    <line x1="8" y1="7" x2="8" y2="11"/>
                    <circle cx="8" cy="5" r=".6" fill="currentColor" stroke="none"/>
                </svg>Info
            </button>
            <button class="btn-edit">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <path d="M11 2.5l2.5 2.5-7 7H4v-2.5l7-7z"/>
                </svg>Edit
            </button>
            <button class="btn-delete">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <polyline points="3,5 13,5"/>
                    <path d="M6 5V3h4v2"/>
                    <path d="M5 5l.7 8h4.6l.7-8"/>
                </svg>Delete
            </button>
        </div>`;
}

// ── Table click delegation ────────────────────
tableBody.addEventListener("click", function (e) {

    // Info
    const infoBtn = e.target.closest(".btn-info");
    if (infoBtn) {
        const userID = infoBtn.closest("tr").cells[0].textContent.trim();
        window.location.href = `/Sections/Users/userInfo.html?id=${userID}`;
        return;
    }

    // Edit
    const editBtn = e.target.closest(".btn-edit");
    if (editBtn) {
        const userID = editBtn.closest("tr").cells[0].textContent.trim();
        window.location.href = `/Sections/Users/editUser.html?id=${userID}`;
        return;
    }

    // Delete
    const deleteBtn = e.target.closest(".btn-delete");
    if (deleteBtn) {
        const row      = deleteBtn.closest("tr");
        const userID   = row.cells[0].textContent.trim();
        const userName = row.cells[3].textContent.trim();

        if(isActive === "false")
        {
            showToast("Your not active user in system, you can't delete any user, please contact with admin to active your account", "error");
            return;
        }
        if (confirm(`Are you sure you want to delete user "${userName}" (ID: ${userID})?`)) {
            deleteUser(userID, row);
        }
    }
});

async function deleteUser(id) {
    try {

        const res = await fetch(`https://localhost:7223/api/Users/Delete/${id}`, { method: "DELETE" });
        if (res.ok) {
            allUsers   = allUsers.filter(u => String(u.userID) !== String(id));
            totalPages = Math.max(1, Math.ceil(allUsers.length / ROWS_PER_PAGE));
            if (currentPage > totalPages) currentPage = totalPages;
            renderPage(currentPage);
            renderPagination();
        } else {
            alert("Failed to delete the user.");
        }
    } catch (err) {
        console.error(err);
        alert("An error occurred while trying to delete.");
    }
}