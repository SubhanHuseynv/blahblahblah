import { useState } from "react";

export default function TodoItem({ todo, onToggle, onUpdate, onDelete }) {
  const [isEditing, setIsEditing] = useState(false);
  const [draft, setDraft] = useState(todo.title);

  const save = async () => {
    const value = draft.trim();
    if (value && value !== todo.title) {
      await onUpdate(todo.id, value, todo.isCompleted);
    } else {
      setDraft(todo.title);
    }
    setIsEditing(false);
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") save();
    if (e.key === "Escape") {
      setDraft(todo.title);
      setIsEditing(false);
    }
  };

  return (
    <li className={`todo-item ${todo.isCompleted ? "done" : ""}`}>
      <input
        type="checkbox"
        checked={todo.isCompleted}
        onChange={() => onToggle(todo.id)}
      />

      {isEditing ? (
        <input
          className="edit-input"
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          onBlur={save}
          onKeyDown={handleKeyDown}
          maxLength={200}
          autoFocus
        />
      ) : (
        <span className="title" onDoubleClick={() => setIsEditing(true)} title="Redaktə üçün iki dəfə klik edin">
          {todo.title}
        </span>
      )}

      <button className="delete" onClick={() => onDelete(todo.id)} aria-label="Sil">
        ✕
      </button>
    </li>
  );
}
