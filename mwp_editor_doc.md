# MVP Documentation – Avalonia Code Editor

This document outlines the tasks, priorities, and expected outcomes for the **minimum viable product (MVP)** of the Avalonia-based code/text editor. The goal is to reach a stage where the editor is fully usable for basic development workflows, with extensions providing syntax highlighting and command support.

---

## Already Completed
- **File Explorer / Tree View**
- **Syntax Highlighting** (via AvaloniaEdit; extensible through extensions)
- **Extension Host & Client System** (with manifest loader)
- **Command Palette** (search/launcher + command binding)
- **Layout & Core UI Framework**
- **Basic Config System** (settings in config files)

---

## MVP Roadmap (Phased)

### **Phase 1 – Core Editing & Files**
- Add **Top Menu Bar** (File, Edit, View, Help).
- File Operations:
  - New File
  - Open File
  - Save / Save As
  - Close File / Close All
- Open Folder / Workspace (set root folder).
- Multi-tab editing (switch between open files).
- Undo / Redo (multi-level history).
- Cut / Copy / Paste.

---

### **Phase 2 – Usability Essentials**
- Find / Replace (in current file).
- Basic Auto-Indent.
- Line Numbers (AvaloniaEdit).
- Status Bar:
  - Line/Column indicator
  - Encoding + EOL style (CRLF/LF)
- Recent Files / Folders (quick access).

---

### **Phase 3 – Extensions & Commands**
- Manual extension management (drop-in folder).
- Extension discovery (scan `/extensions` on startup).
- Load / Unload extensions (without restart if possible).
- Stable API for extensions:
  - Syntax highlighting
  - Commands
- Error handling for bad extensions.
- Add **core editor commands** to command palette (File operations, etc.).
- Keybinding system:
  - Configurable via JSON
  - Default shortcuts (Ctrl+S, Ctrl+P, Ctrl+O, etc.)

---

### **Phase 4 – Settings & Config**
- User settings (`settings.json`).
- Workspace settings (per-project).
- Theme settings (dark/light toggle).
- Keybinding config file.

---

### **Phase 5 – UX Polishing**
- Tabs (closable, draggable if possible).
- Drag & Drop files into editor.
- Basic notifications/toasts (file saved, error, etc.).
- About dialog (app version, author).

---

## Deferred (Post-MVP)
- Git integration.
- Integrated terminal.
- Debugger support.
- Extension marketplace / auto-update.
- Advanced UI customizations (panels, docking, split views).

---

## By MVP Completion, a User Should Be Able To

1. Open a folder/workspace and browse files in a tree view.
2. Open, create, edit, and save files with syntax highlighting.
3. Undo/Redo edits, cut/copy/paste text.
4. Find and replace text in a file.
5. Switch between multiple open files in tabs.
6. View line numbers, cursor position, encoding, and EOL style in the status bar.
7. Quickly reopen recent files/folders.
8. Use the command palette for all core editor functions.
9. Use keyboard shortcuts (Ctrl+S, Ctrl+O, etc.).
10. Load extensions manually from the `~/.config/App/extensions` folder.
11. Run extension-provided syntax highlighting and commands.
12. Edit settings (user/workspace) via JSON.
13. Switch between light and dark themes.
14. Drag and drop files from the system to open them.
15. See notifications (e.g., file saved, extension error).
16. Check the About dialog for app info.

---

## Notes for Developers
- Keep APIs for extensions **minimal and stable** for MVP. Expansion can happen post-MVP.
- Manual extension management is acceptable; marketplace support comes later.
- Focus on **core editing experience** first, since that’s what defines usability.
- Git, terminals, and debugging are out of scope until after MVP.

