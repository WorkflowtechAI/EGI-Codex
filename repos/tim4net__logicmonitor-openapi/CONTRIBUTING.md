# Contributing to LogicMonitor OpenAPI Specification

Thank you for your interest in contributing! This document provides guidelines for contributing to this project.

## Ways to Contribute

### 1. Report Issues

If you find inaccuracies in the specification:

- Missing or incorrect endpoint parameters
- Wrong response schemas
- Outdated endpoint information
- Missing endpoints

Please [open an issue](../../issues/new) with:
- The affected endpoint path
- Expected vs actual behavior
- Evidence (API response, documentation link, etc.)

### 2. Improve Documentation

- Add better descriptions for parameters or schemas
- Include more realistic examples
- Document edge cases or special behaviors

### 3. Add Missing Endpoints

If you discover endpoints not in the spec:

1. Verify the endpoint exists and is documented by LogicMonitor
2. Add the endpoint following existing patterns
3. Include all parameters, responses, and examples

## Making Changes

### Setup

```bash
# Clone the repository
git clone https://github.com/tim4net/logicmonitor-openapi.git
cd logicmonitor-openapi

# Install validation tools (optional)
npm install -g @apidevtools/swagger-cli
```

### Validation

Before submitting changes, validate the spec:

```bash
swagger-cli validate openapi.json
```

### Pull Request Process

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/add-missing-endpoint`)
3. Make your changes
4. Validate the spec
5. Commit with a descriptive message
6. Push to your fork
7. Open a pull request

### Commit Messages

Use clear, descriptive commit messages:

```
Add missing /device/devices/{id}/instances endpoint

- Added GET endpoint for listing device instances
- Included pagination parameters
- Added response schema with example
```

## Style Guidelines

### YAML Formatting

If editing the YAML file:
- Use 2-space indentation
- Keep lines under 120 characters
- Use quotes for strings containing special characters

### Descriptions

- Write clear, concise descriptions
- Use sentence case
- End with periods for full sentences
- Include units where applicable (e.g., "Timeout in seconds")

### Examples

- Provide realistic example values
- Use consistent formatting
- Include examples for complex schemas

## Code of Conduct

- Be respectful and constructive
- Focus on the technical merits
- Welcome newcomers

## Questions?

Open an issue with the "question" label if you need clarification.
