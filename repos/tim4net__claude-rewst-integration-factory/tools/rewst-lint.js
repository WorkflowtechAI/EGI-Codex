#!/usr/bin/env node
/**
 * Rewst OpenAPI Linter
 *
 * Validates OpenAPI specs using the exact same rules as Rewst Custom Integration v2.
 * This replicates the validation from:
 *   packages/engine/engine/agents/kiro/process_openapi_document_request.py
 *
 * Usage:
 *   node rewst-lint.js <spec.json>
 *   npx rewst-lint <spec.json>
 *
 * Requirements:
 *   npm install @stoplight/spectral-cli
 *
 * Exit codes:
 *   0 - Validation passed
 *   1 - Validation errors found
 *   2 - File error (not found, invalid JSON)
 */

const { execSync, spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

const MAX_SIZE_KB = 500;
const RULESET_PATH = path.join(__dirname, '.spectral-rewst.yaml');

// Reserved parameter names that conflict with Rewst internals
const FORBIDDEN_PARAM_NAMES = new Set([
  'json_body', 'body', 'json', 'type', 'url_path', 'method', 'results_key',
  'results', 'data', 'output_schema', 'path_params', 'query_params', 'headers',
  'cookies', 'required', 'parameters', 'response', 'id', 'name', 'parent',
  'parent_id', 'filter', 'filters', 'filter_key', 'filter_keys', 'filter_value',
  'filter_values', 'query', 'queries', 'q'
]);

/**
 * Pre-validation checks (before Spectral)
 * These match Rewst's _lint_document() function
 */
function preValidate(specPath) {
  const errors = [];

  // Check file exists
  if (!fs.existsSync(specPath)) {
    return {
      success: false,
      fatal: `File not found: ${specPath}`,
      errors: []
    };
  }

  // Check file size
  const stats = fs.statSync(specPath);
  const sizeKB = Math.round(stats.size / 1024 * 10) / 10;

  if (sizeKB > MAX_SIZE_KB) {
    errors.push({
      path: '',
      message: `File size ${sizeKB}KB exceeds Rewst's ~${MAX_SIZE_KB}KB limit. Use /rewst-openapi:subset to reduce.`,
      severity: 'error'
    });
  }

  // Parse JSON
  let spec;
  try {
    const content = fs.readFileSync(specPath, 'utf8');
    spec = JSON.parse(content);
  } catch (e) {
    return {
      success: false,
      fatal: 'The OpenAPI document is not valid JSON. Please correct the document and try again.',
      errors: []
    };
  }

  // Check openapi field exists (Rewst check)
  if (!spec.openapi) {
    errors.push({
      path: '$.openapi',
      message: 'The OpenAPI document must have an openapi field.',
      severity: 'error'
    });
  }

  // Check info.title exists (Rewst check)
  if (!spec.info?.title) {
    errors.push({
      path: '$.info.title',
      message: 'The OpenAPI document must have a title under info -> title.',
      severity: 'error'
    });
  }

  return {
    success: errors.length === 0,
    fatal: null,
    errors,
    sizeKB,
    spec
  };
}

/**
 * Rewst-specific validation (warnings)
 * These don't block upload but may cause operations to be skipped or behave unexpectedly
 */
function rewstValidate(spec) {
  const warnings = [];

  if (!spec.paths) return { warnings };

  for (const [pathKey, pathItem] of Object.entries(spec.paths)) {
    const methods = ['get', 'post', 'put', 'delete', 'patch', 'head', 'options', 'trace'];

    for (const method of methods) {
      const operation = pathItem[method];
      if (!operation) continue;

      const opLocation = `paths.${pathKey}.${method}`;

      // Check: Operations without summary AND operationId will be silently skipped
      if (!operation.summary && !operation.operationId) {
        warnings.push({
          path: opLocation,
          message: 'Operation has no summary or operationId - will be SKIPPED by Rewst',
          severity: 'warning'
        });
      }

      // Check: Deprecated operations are skipped
      if (operation.deprecated === true) {
        warnings.push({
          path: opLocation,
          message: 'Deprecated operation will be SKIPPED by Rewst',
          severity: 'warning'
        });
      }

      // Check parameters
      if (operation.parameters) {
        for (let i = 0; i < operation.parameters.length; i++) {
          const param = operation.parameters[i];
          const paramLocation = `${opLocation}.parameters[${i}]`;

          // Check: Cookie parameters are not supported
          if (param.in === 'cookie') {
            warnings.push({
              path: paramLocation,
              message: `Cookie parameter "${param.name}" is NOT SUPPORTED - will be ignored`,
              severity: 'warning'
            });
          }

          // Check: Forbidden parameter names
          if (param.name && FORBIDDEN_PARAM_NAMES.has(param.name.toLowerCase())) {
            warnings.push({
              path: paramLocation,
              message: `Parameter "${param.name}" uses a reserved name - may conflict with Rewst internals`,
              severity: 'warning'
            });
          }
        }
      }
    }
  }

  // Check path-level parameters too
  for (const [pathKey, pathItem] of Object.entries(spec.paths)) {
    if (pathItem.parameters) {
      for (let i = 0; i < pathItem.parameters.length; i++) {
        const param = pathItem.parameters[i];
        const paramLocation = `paths.${pathKey}.parameters[${i}]`;

        if (param.in === 'cookie') {
          warnings.push({
            path: paramLocation,
            message: `Cookie parameter "${param.name}" is NOT SUPPORTED - will be ignored`,
            severity: 'warning'
          });
        }

        if (param.name && FORBIDDEN_PARAM_NAMES.has(param.name.toLowerCase())) {
          warnings.push({
            path: paramLocation,
            message: `Parameter "${param.name}" uses a reserved name - may conflict with Rewst internals`,
            severity: 'warning'
          });
        }
      }
    }
  }

  return { warnings };
}

/**
 * Run Spectral with Rewst's ruleset
 */
function runSpectral(specPath) {
  return new Promise((resolve) => {
    const args = [
      'lint',
      specPath,
      '--ruleset', RULESET_PATH,
      '--format', 'json'
    ];

    // Try npx first, fall back to global
    let command = 'npx';
    let fullArgs = ['@stoplight/spectral-cli', ...args];

    const proc = spawn(command, fullArgs, {
      stdio: ['pipe', 'pipe', 'pipe'],
      shell: true
    });

    let stdout = '';
    let stderr = '';

    proc.stdout.on('data', (data) => { stdout += data; });
    proc.stderr.on('data', (data) => { stderr += data; });

    proc.on('close', (code) => {
      let results = [];
      try {
        if (stdout.trim()) {
          results = JSON.parse(stdout);
        }
      } catch (e) {
        // If JSON parse fails, check if Spectral isn't installed
        if (stderr.includes('not found') || stderr.includes('ENOENT')) {
          resolve({
            success: false,
            fatal: 'Spectral not installed. Run: npm install -g @stoplight/spectral-cli',
            errors: []
          });
          return;
        }
      }

      const errors = results
        .filter(r => r.severity === 0) // 0 = error in Spectral
        .map(r => ({
          path: r.path?.join('.') || '',
          message: r.message,
          severity: 'error',
          code: r.code,
          line: r.range?.start?.line
        }));

      const warnings = results
        .filter(r => r.severity === 1) // 1 = warning
        .map(r => ({
          path: r.path?.join('.') || '',
          message: r.message,
          severity: 'warning',
          code: r.code,
          line: r.range?.start?.line
        }));

      resolve({
        success: errors.length === 0,
        errors,
        warnings
      });
    });

    proc.on('error', (err) => {
      resolve({
        success: false,
        fatal: `Failed to run Spectral: ${err.message}`,
        errors: []
      });
    });
  });
}

/**
 * Format output
 */
function formatOutput(result, specPath, jsonOutput) {
  if (jsonOutput) {
    return JSON.stringify(result, null, 2);
  }

  const lines = [];
  const filename = path.basename(specPath);

  lines.push('');
  lines.push(`Rewst OpenAPI Linter - ${filename}`);
  lines.push('='.repeat(50));

  if (result.sizeKB) {
    lines.push(`Size: ${result.sizeKB} KB (limit: ~${MAX_SIZE_KB} KB)`);
  }
  lines.push('');

  if (result.fatal) {
    lines.push(`FATAL: ${result.fatal}`);
    return lines.join('\n');
  }

  const errorCount = result.errors?.length || 0;
  const warningCount = result.warnings?.length || 0;

  if (errorCount === 0) {
    lines.push('✓ PASSED - Ready for Rewst upload');
    if (warningCount > 0) {
      lines.push(`  (${warningCount} warning${warningCount > 1 ? 's' : ''} - optional to fix)`);
    }
  } else {
    lines.push(`✗ FAILED - ${errorCount} error${errorCount > 1 ? 's' : ''} must be fixed`);
  }
  lines.push('');

  if (result.errors?.length > 0) {
    lines.push('Errors:');
    lines.push('-'.repeat(40));
    for (const err of result.errors) {
      lines.push(`✗ ${err.message}`);
      if (err.path) lines.push(`  at: ${err.path}`);
      if (err.code) lines.push(`  rule: ${err.code}`);
      lines.push('');
    }
  }

  if (result.warnings?.length > 0 && !jsonOutput) {
    lines.push('Warnings (optional):');
    lines.push('-'.repeat(40));
    for (const warn of result.warnings.slice(0, 5)) {
      lines.push(`⚠ ${warn.message}`);
      if (warn.path) lines.push(`  at: ${warn.path}`);
      lines.push('');
    }
    if (result.warnings.length > 5) {
      lines.push(`  ... and ${result.warnings.length - 5} more warnings`);
      lines.push('');
    }
  }

  return lines.join('\n');
}

/**
 * Main
 */
async function main() {
  const args = process.argv.slice(2);

  if (args.length === 0 || args.includes('--help') || args.includes('-h')) {
    console.log(`
Rewst OpenAPI Linter

Validates specs using Rewst's exact validation rules.

Usage: node rewst-lint.js <spec.json> [options]

Options:
  --json    Output as JSON
  --help    Show this help

Requirements:
  npm install -g @stoplight/spectral-cli

This linter replicates Rewst's validation exactly, so specs that
pass here should upload successfully to Rewst CI v2.
`);
    process.exit(0);
  }

  const specPath = args.find(a => !a.startsWith('--'));
  const jsonOutput = args.includes('--json');

  if (!specPath) {
    console.error('Error: No spec file provided');
    process.exit(2);
  }

  // Run pre-validation (JSON, openapi field, title)
  const preResult = preValidate(specPath);

  if (preResult.fatal) {
    console.log(formatOutput(preResult, specPath, jsonOutput));
    process.exit(2);
  }

  // Run Spectral with Rewst's ruleset
  const spectralResult = await runSpectral(specPath);

  if (spectralResult.fatal) {
    console.log(formatOutput(spectralResult, specPath, jsonOutput));
    process.exit(2);
  }

  // Run Rewst-specific validation (generates warnings)
  const rewstResult = rewstValidate(preResult.spec);

  // Combine results
  const combined = {
    success: preResult.success && spectralResult.success,
    sizeKB: preResult.sizeKB,
    errors: [...preResult.errors, ...(spectralResult.errors || [])],
    warnings: [...(spectralResult.warnings || []), ...(rewstResult.warnings || [])]
  };

  console.log(formatOutput(combined, specPath, jsonOutput));
  process.exit(combined.success ? 0 : 1);
}

main().catch(err => {
  console.error('Unexpected error:', err);
  process.exit(2);
});
