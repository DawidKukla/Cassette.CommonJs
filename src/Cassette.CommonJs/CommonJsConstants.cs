using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassette.CommonJs
{
  internal class CommonJsConstants
  {
    public const string Prelude = @"(function (global, globals, entries) {
  var originalRequire = typeof require === 'function' && require;
  var modules = {};

  function loadModule (entry)
  {
    var m = { exports: {} };
    entry.body(localRequire.bind(m.exports, entry.refs), m, m.exports);
    return (modules[entry.path] = m.exports);
  }

  function findEntry (path)
  {
    for (var i = 0, ln = entries.length; i < ln; i++)
    {
      if (path === entries[i].path)
      {
        return entries[i];
      }
    }

    return null;
  }

  function localRequire (localMap, path)
  {
    if (globals && path in globals)
    {
      return global[globals[path]];
    }

    if (path[path.length - 1] === '/')
    {
      path += 'index';
    }

    if (localMap && path in localMap)
    {
      path = localMap[path];
    }

    if (path in modules)
    {
      return modules[path];
    }

    var entry = findEntry(path);
    if (entry)
    {
      return loadModule(entry);
    }

    if (originalRequire)
    {
      return originalRequire(path, true)
    }

    throw Error('Unable to find module: ' + path);
  }

  // load all modules
  entries.forEach(function (entry) {
    if (!(entry.path in modules))
    {
      loadModule(entry);
    }
  });
})";
  }
}
