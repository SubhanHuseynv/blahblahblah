const BASE_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5000/api";

async function request(path, options = {}) {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!res.ok) {
    const body = await res.json().catch(() => null);
    throw new Error(body?.message ?? body?.title ?? `Xəta: ${res.status}`);
  }

  return res.status === 204 ? null : res.json();
}

export const todoApi = {
  getAll: () => request("/todos"),
  create: (title) => request("/todos", { method: "POST", body: JSON.stringify({ title }) }),
  update: (id, title, isCompleted) =>
    request(`/todos/${id}`, { method: "PUT", body: JSON.stringify({ title, isCompleted }) }),
  toggle: (id) => request(`/todos/${id}/toggle`, { method: "PATCH" }),
  remove: (id) => request(`/todos/${id}`, { method: "DELETE" }),
};
