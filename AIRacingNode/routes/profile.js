var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', isLoggedIn, function(req, res) {
    var scripts = req.db.get('scriptcollection');
    var times = req.db.get('timecollection');

    var scores = [];

    // Create a new object to keep track of how much async work is done, and what to do when all work is complete
    var work = new Work(function() { res.render('profile', { "title": "Profile", "scores": scores }); });

    find(scripts, {email: req.user.local.email}, work, function(scriptDoc) {
        find(times, {scriptName: scriptDoc.scriptName}, work, function(timeDoc) {
            scores.push({
                time       : timeDoc.time,
                scriptName : timeDoc.scriptName,
                levelName  : scriptDoc.levelName
            });
        });
    });
});

// Calls BODY(doc) for each document in COLLECTION that matches FILTER.
// Uses work to track async tasks remaining
function find(collection, filter, work, body) {
    work.start();

    collection.find(filter, {}, function(e, docs) {
        for (var i = 0; i < docs.length; i++) {
            body(docs[i]);
        }

        work.end();
    });
}

function Work(onComplete) {
    this.count = 0;
    this.onComplete = onComplete;
    this.end = function() { if(--this.count == 0) this.onComplete(); };
    this.start = function() { this.count++ };
}

function isLoggedIn(req, res, next) {

  req.session.redirect = '/profile';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
