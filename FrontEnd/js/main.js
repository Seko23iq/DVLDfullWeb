// ── Username ───────────────────────────────────────────────────────
document.addEventListener("layoutReady", () => {
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");
    const storedImage = localStorage.getItem("UserImage"); // ← مسار الصورة

    if (userNameElement) {
        userNameElement.textContent = storedName || "Guest";
    }

        
    const imagePath = localStorage.getItem("ImagePath");
    if (imagePath) {
        const img = document.getElementById("avatar-img");
        const initials = document.getElementById("avatar-initials");
        img.src = "https://localhost:7223" + imagePath;
        img.style.display = "block";
        initials.style.display = "none";
    }

    // ── Auto-open parent dropdown if child link is active ──────────
    document.querySelectorAll(".nav-dropdown").forEach(dropdown => {
        const hasActive = dropdown.querySelector(".nav-submenu a.active");
        if (hasActive) {
            dropdown.classList.add("open");
            dropdown.querySelector(".nav-dropdown-toggle").classList.add("active");
        }
    });

    // ── Auto-open sub-dropdown if grandchild link is active ────────
    document.querySelectorAll(".nav-sub-dropdown").forEach(sub => {
        const hasActive = sub.querySelector("a.active");
        if (hasActive) {
            sub.classList.add("open");
        }
    });
});

// ── Dropdown toggle ────────────────────────────────────────────────
function toggleDropdown(id) {
    const dropdown = document.getElementById(id);
    const isOpen = dropdown.classList.contains("open");

    // Close all top-level dropdowns first
    document.querySelectorAll(".nav-dropdown.open").forEach(d => d.classList.remove("open"));

    if (!isOpen) {
        dropdown.classList.add("open");
    }
}

function toggleSubDropdown(id, event) {
    event.stopPropagation();
    const el = document.getElementById(id);
    el.classList.toggle("open");
}
