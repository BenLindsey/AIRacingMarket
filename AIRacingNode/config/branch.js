// config/branch.js
// This module converts a given branch name into a port.
//
// The mapping branch -> port is stored in mongoDB(redirectcollection)
// If no mapping exists, one is created and the apache2 redirection service is updated & restarted.

module.exports = function(db, branch, portHandle) {
    // Set default port
    var port = 3001;

    var collection = db.get('redirectcollection');

    collection.find({}, {}, function(e, docs) {
        var found = false;

        // Find the existing mapping, or else find the next available port
        docs.forEach(function(doc) {
            if(doc.branch == branch) {
                found = true;
                port = doc.port;
            }

            if(!found && doc.port >= port) {
                port = doc.port + 1;
            }
        });

        // Create a new redirection if none exists
        if(!found) {
            // Update Mongo
            collection.insert({"branch" : branch, "port" : port});

            // Add the new redirection to apache2's config file (.htaccess)
            require('fs').appendFileSync('/var/www/html/.htaccess',
                    '\nRewriteEngine on\nRewriteRule ^' + branch +'$ http://146.169.47.15:' + port + ' [NC]');

            // Restart apache2 so it reads the updated config file
            require('child_process').exec('sudo service apache2 restart',
                function callback(error, stdout, stderr){ });

            console.log("Created port " + port);
        }

        // Call the given callback with the found/created port (e.g. start the server with this port)
        portHandle(port);
    });
};