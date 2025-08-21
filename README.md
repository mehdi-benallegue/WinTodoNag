# DRAFT PROJECT

# WinTodoNag
This repository currently contains an empty WPF project structure.

**WinTodoNag** is a standalone Windows task management app with hierarchical tasks, persistent reminders, and a focus on being “usefully annoying.”
It is designed to help you actually *do* your tasks instead of forgetting them.

## Features

* **Hierarchical tasks**

  * Add tasks with unlimited subtasks.
  * Recursive check/uncheck:

    * Checking a parent marks all subtasks as done (recursively).
    * Subtasks remember whether they were completed manually or recursively.
    * Unchecking a parent only reopens subtasks that were auto-completed.

* **Two views**

  * **List tab:** Sorted by the next notification time.
  * **Calendar tab:** Monthly grid with deadlines and reminders.

* **Reminders that won’t let you forget**

  * Choose between **Windows toast notifications** or **nag dialogs**.
  * Toasts re-fire after 1 minute if ignored.
  * Nag dialogs stay on top but don’t steal focus. If closed without action, they reappear after 1 minute.

* **Snooze options**

  * Presets: 2, 5, 10, 15 minutes; 1, 2, 4, 8 hours; 1, 2, 4 days; 1, 2, 3 weeks; 1, 2, 3 months.
  * Custom snooze with date/time picker.
  * Clear preview before confirming: “Will ring again in X on Y.”

* **Task metadata**

  * Each task stores creation date, deadline, first notification, next notification, and completion date.

* **Data storage**

  * Human-readable **YAML file** you can choose the location of.
  * Rotating backups to avoid data loss.

* **Background-friendly**

  * Runs in the system tray.
  * Starts automatically with Windows (optional).
  * Single-instance enforced.

## Tech Stack

* **.NET 8 / C#** with **WPF** GUI.
* **YamlDotNet** for storage.
* **CommunityToolkit.WinUI.Notifications** for Windows toasts.

## Getting Started

1. Clone the repo.

2. Install [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

3. Build and run:

   ```sh
   dotnet build
   dotnet run
   ```

4. On first launch, choose a YAML file location to store your tasks.

## Roadmap

* [ ] Real Windows toast actions (Snooze / Complete buttons).
* [ ] Dedicated custom snooze dialog.
* [ ] More polished calendar interaction (drag & drop to reschedule).
* [ ] Tags, colors, and filters.

## License

Open source, MIT License.


