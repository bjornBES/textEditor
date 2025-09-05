# MVP Dev Roadmap (Phased Checklist)

## **Phase 1 – Core Editing & Files (highest priority)**

* [X] **Top Menu Bar** (File, Edit, View, Help).
* [X] File Operations:

  * [X] New File
  * [X] Open File
  * [X] Save / Save As
* [X] Open Folder / Workspace (set root folder).
  * [X] Also as a tab bar button.
* [X] Multi-tab editing (switch between open files).
* [X] Close tabs.
* [X] Close all tabs.
* [X] Undo / Redo (multi-level history).
* [X] Cut / Copy / Paste.
* [ ] Basic systax highlighing
  * [ ] an API for systax highlighing for TextMate

---

## **Phase 2 – Usability Essentials**

* [ ] Find / Replace (in current file).
* [ ] Basic Auto-Indent.
* [X] Line Numbers (AvaloniaEdit).
* [ ] Status Bar:
  * [ ] Line/Column indicator
  * [ ] Encoding + EOL style (CRLF/LF)
* [/] Recent Files / Folders (quick access).
* [ ] Working themes
* [ ] Tab handling
  * [ ] Pinning tabs
  * [ ] Tab overflow handling
* [ ] File renaming and deletion from explorer
* [ ] Bracket/parenthesis matching
* [ ] Start menu

---

## **Phase 3 – Extensions & Commands**

* [ ] Manual extension management (drop-in folder).
* [ ] Extension discovery (scan `~/.config/app/extensions` on startup).
  * [ ] Add a new tab called Extensions to "manage" extensions and to open the `/extensions` folder
* [ ] Load / Unload extensions (without restart if possible).
* [ ] Stable API for extensions:
  * [ ] Syntax highlighting
  * [ ] Commands
* [ ] Error handling for bad extensions.
* [/] Add **core editor commands** to command palette (File operations, etc.).
* [ ] Keybinding system:
  * [ ] Configurable via JSON
  * [ ] Default shortcuts (Ctrl+S, Ctrl+P, Ctrl+O, etc.)

---

## **Phase 4 – Settings & Config**

* [ ] User settings (`settings.json`).
* [ ] Workspace settings (per-project).
* [ ] Theme settings (dark/light toggle).
* [ ] Keybinding config file.

---

## **Phase 5 – UX Polishing**

* [ ] Tabs (closable, draggable if possible).
* [ ] Drag & Drop files into editor.
* [ ] Basic notifications/toasts (file saved, error, etc.).
* [ ] About dialog (app version, author).
