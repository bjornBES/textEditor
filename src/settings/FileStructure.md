# File Structure

The settings will be store in jsons

The typical location for the files are

## User Settings / Keybindings / Snippets

### Windows

``` plantext
%APPDATA%\[TEXT EDITOR]\User
```

### Linux

``` plantext
~/.config/[TEXT EDITOR]/User
```

### macOS

There is not suport for [TEXT EDITOR] right now

## Workspace / Project Settings

### Local

Projects can have project-specific override settings from the global settings
these are stored under

``` plantext
.editor/settings.json
```

### Global

### Windows

``` plantext
%APPDATA%\[TEXT EDITOR]\settings.json
```

### Linux

``` plantext
~/.config/[TEXT EDITOR]/settings.json
```

## Extensions

Extensions are installed to these paths

### Windows

``` plantext
%USERPROFILE%\.[TEXT EDITOR]\extensions
```

### Linux

``` plantext
~/.[TEXT EDITOR]/extensions/
```
