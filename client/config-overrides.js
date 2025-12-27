module.exports = function override(config) {
  config.resolve.fallback = {
    ...config.resolve.fallback,
    path: false,
    fs: false,
    http: false,
    https: false,
    stream: false,
    crypto: false,
    zlib: false,
    url: false,
    buffer: false,
    util: false,
    querystring: false,
    async_hooks: false,
  };

  config.ignoreWarnings = [/Failed to parse source map/];

  return config;
};
