# Contributing to ProfHeat 🤝

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

## Getting Started using the CLI 🛠️

For a smooth collaboration process, use Git for version control and the GitHub CLI for managing pull requests and issues. This guide assumes you have write access to the repository. To contribute to this project, follow these steps:
- **Note:** If you don't have write access, please fork the repository first and then follow these instructions on your fork.

1. **Clone:** Clone the repository to your local machine:
   ```sh
   git clone https://github.com/SP-SoftFuzz/ProfHeat.git
   ```
2. **Branch**: Create a new branch for your feature. Switch to the `main` branch, then create and switch to a new branch named after the JIRA issue key and feature, e.g., `SCRUM-123-Feature`:
   ```sh
   git checkout main
   git checkout -b SCRUM-123-Feature
   ```
3. **Commit:** Make your changes locally. Once you're satisfied, commit them with a clear message that includes the JIRA issue key:
   ```sh
   git add .
   git commit -m "SCRUM-123: Add some AmazingFeature"
   ```
4. **Push:** Push your new branch and changes to GitHub:
   ```sh
   git push -u origin SCRUM-123-Feature
   ```
5. **Pull Request:** Use the GitHub CLI to create a pull request against the `main` branch. Include the JIRA issue key in the title of your pull request:
   ```sh
   gh pr create --base main --head SCRUM-123-Feature --title "SCRUM-123: Added Feature" --body "Detailed description of the changes."
   ```
6. **Request a Review:** After creating your pull request, request a review from a teammate using the GitHub website or CLI:
   ```sh
   gh pr review --request @[your-teammate] --pr SCRUM-123-Feature
   ```
7. **Stacking:** To continue working on another feature based on your current feature work, create a new branch off your **current feature branch** instead of `main`. This allows you to build upon your current work without waiting for it to be merged:
   ```sh
   git checkout SCRUM-123-Feature
   git checkout -b SCRUM-124-NextFeature
   ```

Please ensure that your pull request adheres to the following guidelines:
- Provide a descriptive title and summary of the changes.
- If applicable, reference related issues or pull requests.
- Make sure your code is properly formatted and follows the [Google C# Style Guide](https://google.github.io/styleguide/csharp-style.html).
- Ensure any install or build dependencies are removed before submitting the pull request.
- Update the README.md with any changes to the project's functionality.

## Code of Conduct 📝

Please note that this project is released with a [Contributor Code of Conduct](CODE_OF_CONDUCT.md). By participating in this project you agree to abide by its terms.

## GitHub Flow 🌊

This project follows the [GitHub Flow](https://docs.github.com/en/get-started/using-github/github-flow). Make sure you're familiar with it before contributing.

