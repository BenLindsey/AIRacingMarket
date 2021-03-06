var express = require('express');
var router = express.Router();
var defaults = require('../config/defaults');

/* GET user profile. */
router.get('/', [isLoggedIn, isAdmin], function(req, res) {
    var collection = req.db.get('users');
    var scriptcollection = req.db.get('scriptcollection');

    collection.find({}, {sort : { email : 1 }}, function(e, docs) {

        console.log("Retrieve documents: ");
        console.log(docs);

        scriptcollection.find({}, {}, function(e, scriptdocs) {

            console.log("Script docs: ");
            console.log(scriptdocs);

            docs.push({local:{}});

            // Append scripts for each email to the retrieved documents.
            scriptdocs.forEach(function (scriptdoc) {
               for (var i = 0; i < docs.length; i++) {
                   if (scriptdoc.email === docs[i].local.email) {
                       // Create scrips array if necessary.
                       if (!docs[i].local.scripts) {
                           docs[i].local.scripts = [];
                       }
                       docs[i].local.scripts.push(scriptdoc.scriptName);
                   }
               }
               
            });

            docs[docs.length-1].local.email = defaults.anon;

            res.render('admin', {
                "users"    : docs,
                "defaults" : defaults
            });
        });
    });
});

router.post('/deleteuser', [isLoggedIn, isAdmin], function(req, res) {
    var collection = req.db.get('users');
   
    console.log("deleting " + req.body.user); 
    collection.remove({"local.email":req.body.user}, function (err, doc) {
        if (err) {
            res.send("There was a problem deleting user");
        } else {
           res.sendStatus(200);
        }
    });
});

router.post('/deletescripts', [isLoggedIn, isAdmin], function(req, res) {
    var collection = req.db.get('scriptcollection');
   
    console.log("deleting scripts for " + req.body.user);
    var email = req.body.user === defaults.anon ? null : req.body.user;
    collection.remove({"email":email}, function (err, doc) {
        if (err) {
            res.send("There was a problem deleting scripts");
        } else {
           res.sendStatus(200);
        }
    });
});

router.post('/deletescript', [isLoggedIn, isAdmin], function(req, res) {
    var collection = req.db.get('scriptcollection');
   
    console.log("deleting scripts for " + req.body.user); 
    collection.remove({"scriptName":req.body.script}, function (err, doc) {
        if (err) {
            res.send("There was a problem deleting script");
        } else {
           res.sendStatus(200);
        }
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
