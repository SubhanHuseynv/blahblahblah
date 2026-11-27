import { useEffect, useState } from "react";
import { todoApi } from "./api/todoApi";
import TodoForm from "./components/TodoForm";
import TodoItem from "./components/TodoItem";

const FILTERS = {
  all: { label: "Hamısı", fn: () => true },
  active: { label: "Aktiv", fn: (t) => !t.isCompleted },
  done: { label: "Bitmiş", fn: (t) => t.isCompleted },
};

export default function App() {
  const [todos, setTodos] = useState([]);
  const [filter, setFilter] = useState("all");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const run = async (action) => {
    try {
      setError("");
      await action();
    } catch (err) {
      setError(err.message);
    }
  };

  useEffect(() => {
    todoApi
      .getAll()
      .then(setTodos)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  const handleAdd = (title) =>
    run(async () => {
      const created = await todoApi.create(title);
      setTodos((prev) => [created, ...prev]);
    });

  const handleToggle = (id) =>
    run(async () => {
      await todoApi.toggle(id);
      setTodos((prev) => prev.map((t) => (t.id === id ? { ...t, isCompleted: !t.isCompleted } : t)));
    });

  const handleUpdate = (id, title, isCompleted) =>
    run(async () => {
      await todoApi.update(id, title, isCompleted);
      setTodos((prev) => prev.map((t) => (t.id === id ? { ...t, title } : t)));
    });

  const handleDelete = (id) =>
    run(async () => {
      await todoApi.remove(id);
      setTodos((prev) => prev.filter((t) => t.id !== id));
    });

  const visible = todos.filter(FILTERS[filter].fn);
  const activeCount = todos.filter((t) => !t.isCompleted).length;

  return (
    <main className="container">
      <h1>📝 Todo App</h1>

      <TodoForm onAdd={handleAdd} />

      {error && <p className="error">{error}</p>}

      <div className="filters">
        {Object.entries(FILTERS).map(([key, { label }]) => (
          <button key={key} className={filter === key ? "active" : ""} onClick={() => setFilter(key)}>
            {label}
          </button>
        ))}
      </div>

      {loading ? (
        <p className="muted">Yüklənir...</p>
      ) : visible.length === 0 ? (
        <p className="muted">Heç bir tapşırıq yoxdur.</p>
      ) : (
        <ul className="todo-list">
          {visible.map((todo) => (
            <TodoItem
              key={todo.id}
              todo={todo}
              onToggle={handleToggle}
              onUpdate={handleUpdate}
              onDelete={handleDelete}
            />
          ))}
        </ul>
      )}

      <footer className="muted">{activeCount} aktiv tapşırıq qalıb</footer>
    </main>
  );
}
