// 1) Booking form: load free time slots with fetch() whenever the attorney or date changes.
(function () {
  const date = document.getElementById("Date");
  const time = document.getElementById("Time");
  const lawyer = document.getElementById("LawyerId");
  if (!date || !time) return;

  const preselected = time.dataset.selected || "";

  function fill(options, placeholder) {
    time.innerHTML = "";
    const first = document.createElement("option");
    first.value = "";
    first.textContent = placeholder;
    time.appendChild(first);
    options.forEach(function (slot) {
      const o = document.createElement("option");
      o.value = slot;
      o.textContent = slot;
      if (slot === preselected) o.selected = true;
      time.appendChild(o);
    });
    time.disabled = options.length === 0;
  }

  async function loadSlots() {
    if (!date.value) {
      fill([], "Choose a date first");
      return;
    }
    fill([], "Loading times...");
    const params = new URLSearchParams({ date: date.value });
    if (lawyer && lawyer.value) params.set("lawyerId", lawyer.value);

    try {
      const response = await fetch("/Appointment/Slots?" + params.toString());
      if (!response.ok) throw new Error("Request failed");
      const slots = await response.json();
      fill(slots, slots.length ? "Choose a time" : "No times available that day");
    } catch (e) {
      fill([], "Could not load times. Try again.");
    }
  }

  date.addEventListener("change", loadSlots);
  if (lawyer) lawyer.addEventListener("change", loadSlots);
  loadSlots();
})();

// 2) Ask before running destructive actions (forms marked with data-confirm).
document.addEventListener("submit", function (e) {
  const message = e.target.getAttribute("data-confirm");
  if (message && !window.confirm(message)) e.preventDefault();
});
