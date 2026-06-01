
// ── Dropdown toggle ────────────────────────────────────────────
export function toggleDropdown(id) {
    const dropdown = document.getElementById(id);
    const isOpen = dropdown.classList.contains("open");
    // Close all dropdowns first
    document.querySelectorAll(".nav-dropdown.open").forEach(d => d.classList.remove("open"));
    // Toggle the clicked one
    if (!isOpen) {
        dropdown.classList.add("open");
    }
}