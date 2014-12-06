var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', [isLoggedIn, isAdmin], function(req, res) {
    var collection = req.db.get('users');
    var scriptcollection = req.db.get('scriptcollection');

    collection.find({}, {sort : { email : 1 }}, function(e, docs) {

        // Make a list of all emails, and create an array for the scripts to be stored.
        var emails = [];
        for (var i = 0; i < docs.length; i++) {
            docs[i].scripts = [];
            emails.append(docs[i].local.email);
        };

        scriptcollection.find({email: { $in: emails}}, {}, function(e, scriptdocs) {
            // Append scripts for each email to the retrieved documents.
            scriptdocs.forEach(function (scriptdoc) {
               for (var i = 0; i < docs.length; i++) {
                   if (scriptdoc.email === docs[i].local.email) {
                       docs[i].local.scripts.append(scriptdoc.scriptName);
                   }
               }
            });

            // I miss SQL. I never thought I'd say it.

            res.render('admin', {
                "users" : docs
            });
        });
    });
});

function isLoggedIn(req, res, next) {

    req.session.redirect = '/admin';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

function isAdmin(req, res, next) {
    if (req.user && req.user.local.admin)
        return next();

    res.send(401, 'Unauthorized');
}

module.exports = router;
