'use strict';

// postinstall entry point. Downloads the platform binary up-front so the first
// `officecli` call is instant. A failure here is non-fatal: the bin shim
// (bin/officecli.js) lazily downloads on first run, so an offline/proxied
// install still leaves a working command once connectivity returns. Set
// OFFICECLI_SKIP_BINARY_DOWNLOAD=1 to skip the download entirely.

// Secured fork: neutralize F-002 (npm postinstall download-and-execute).
// This early exit prevents all download-and-execute logic below from running.
process.stderr.write('[officecli] This secure fork does not distribute binaries. Build from source.\n');
process.exit(0);

if (process.env.OFFICECLI_SKIP_BINARY_DOWNLOAD) {
  process.stderr.write('[officecli] OFFICECLI_SKIP_BINARY_DOWNLOAD set, skipping binary download.\n');
  process.exit(0);
}

require('./lib/install-binary')
  .ensureBinary()
  .catch(function (err) {
    process.stderr.write('[officecli] postinstall could not fetch the binary: ' + err.message + '\n');
    process.stderr.write('[officecli] it will be downloaded on first run instead.\n');
    // Exit 0 so `npm install` succeeds; the shim retries lazily.
    process.exit(0);
  });
