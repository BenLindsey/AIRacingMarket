// Setup the database with:
// -- Basic scripts.
// -- Default admin account

module.exports = function(db) {
    setupScripts(db);
};

function setupScripts(db) {
    var glob = require('glob');
    var scriptCollection = db.get('scriptcollection');

    scriptCollection.count({}, function(err, count) {
        if (count === 0) {
            glob('scripts/*', function (err, scriptfiles) {
                var scripts = [];

                scriptfiles.forEach(function (scriptfile) {
                    scripts.push(require('../' + scriptfile));
                });

                scriptCollection.insert(scripts);
            });
        }
    });
}