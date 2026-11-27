import { useState } from "react";

export default function TodoForm({ onAdd }) {
  const [title, setTitle] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    const value = title.trim();
    if (!value) return;
    await onAdd(value);
    setTitle("");
  };

  return (
    <form className="todo-form" onSubmit={handleSubmit}>
      <input
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="Nə etmək lazımdır?"
        maxLength={200}
        autoFocus
      />
      <button type="submit" disabled={!title.trim()}>
        Əlavə et
      </button>
    </form>
  );
}
