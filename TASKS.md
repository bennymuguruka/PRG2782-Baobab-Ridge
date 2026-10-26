# Baobab Ridge implementation checklist

Source of truth: `PRG2782 Project 2026.pdf`; implementation rules: `AGENTS.md`.
Status: `[ ]` not started · `[~]` in progress · `[x]` verified complete · `[!]` blocked

**Smallest sensible sequence:** migrate the blank project to .NET 8 and the agreed folders; implement model/classification; implement validation and file access; build View/Add; then Update/Delete; then Summary; finish error cases and verification. Keep the form, model, repository and two services in the supplied layout; add no further layers unless needed.

## 1. Animal model and classification
- [~] **Model/project** — Acceptance: convert the existing .NET Framework 4.7.2 project to .NET 8+ Windows Forms; place the main form and `Animal` class in `Forms/` and `Models/`; project builds. `Animal` is in `Models/`; framework migration and form move remain.
- [~] **Classification** — Acceptance: one shared method in `AnimalService` returns the exact status and housing for all five score bands; Add and Update both use it. Classifier is implemented; Add/Update callers remain.

## 2. Text-file persistence and validation
- [~] **File repository** — Acceptance: `AnimalFileRepository` reads/writes UTF-8 `animals.txt` beside the executable, uses the seven-field format, creates a missing file, preserves order, skips malformed lines and reports the skipped count once. Include 8+ valid starter records across 4+ statuses and copy if newer. Implemented; build passes, runtime file/error cases remain to verify.
- [~] **Input validation** — Acceptance: trim inputs; enforce required ID format/case/uniqueness on Add, immutable ID on Update, name/species limits and pipe restriction, and whole-number age/score ranges. Invalid input saves nothing and identifies/focuses the field. Add validation passed focused checks; Update reuse and interactive focus checks remain.

## 3. View and Add features
- [~] **View** — Acceptance: startup grid is read-only, whole-row selecting, correctly ordered, preserves file order, and refreshes without duplicate rows. Implemented; verify interaction and repeated refreshes at runtime.
- [~] **Add** — Acceptance: labelled inputs append one valid record; show classification; refresh the grid and clear inputs. Implemented; validation/append checks pass; interactive dialog, focus, refresh and clear behavior remain to verify.

## 4. Update and Delete features
- [~] **Update** — Acceptance: grid selection and exact case-insensitive ID search load the same inputs; edit controls behave as specified; save changes only that record and recalculates classification. Implemented and service/repository checks pass; interactive form state and focus behavior remain to verify.
- [~] **Delete** — Acceptance: require a selected row; confirm ID/name and irreversibility; No changes nothing; Yes removes only that record, refreshes, and clears a matching loaded form. Removal and file cases pass focused checks; interactive confirmation and loaded-form clearing remain to verify.

## 5. Summary report
- [~] **Report** — Acceptance: `ReportService` calculates totals, averages and all status counts; display results on the form; overwrite `summary.txt` beside the executable in the specified layout and invariant number format; empty data shows `N/A` averages and zero counts. Report formatting, empty-data, overwrite and file-error checks pass; interactive form display remains to verify.

## 6. Error handling
- [ ] **File and normal-use errors** — Acceptance: handle `IOException` and `UnauthorizedAccessException` for reads, appends, rewrites and reports with friendly messages and no crash; cover empty inputs/files and Save/Delete without selection. No stack traces.

## 7. Testing and final verification
- [ ] **Build and behavior** — Acceptance: build/run from a fresh clone; verify all five features, validation failures, classification boundaries (0, 19, 20, 39, 40, 59, 60, 79, 80, 100), missing/empty/malformed data, locked/read-only files, file formats, and no duplicate rows. For Add, verify field-specific messages/focus, invalid records are not appended, successful form clearing and grid refresh. For Update, verify search failure leaves the form unchanged, selection/search enter the same edit state, Clear restores Add mode, invalid input is not written, unchanged scores are reclassified, boundary edits update status/housing, and other file records remain unchanged. For Delete, verify no-selection prompt, Yes/No confirmation, cancellation, final-record deletion, unchanged remaining records, grid refresh, clearing a loaded record, and file-error messages. For Summary, verify exact report text, timestamps, invariant averages, all zero-count status lines, empty input, overwrite behavior, on-form display and file errors. For persistence/View also verify one skipped-line notification with the exact count, access-denied and locked-file messages, starter file copy-if-newer behavior, and repeated grid refreshes. Also test invalid scores -1/101, invalid ID formats and lowercase normalization, blank/31-character names and species, pipe characters, and decimal/text age or score input.
- [ ] **Submission review** — Acceptance: README lists project, member names/numbers, build/run steps, features, formats and limitations; generated/build files and machine-specific paths are excluded; record verification evidence and anything not verified. Follow genuine Git/PR requirements; do not fabricate history or contributions.

## Unresolved
- The PDF requires 3–4 students from the same class/module type; the identified member list currently contains only Mugisho Benny Muguruka (603643). Confirm additional members or a lecturer-approved exception before treating group requirements as satisfied.
- Confirm with the lecturer what AI assistance is permitted; do not submit AI-generated code or text as the group's own work.
