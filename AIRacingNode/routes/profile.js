var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', isLoggedIn, function(req, res) {
    var scripts = req.db.get('scriptcollection');
    var times = req.db.get('timecollection');

    var scores = [];

    findAndCheckIfFinal(scripts, {email: req.user.local.email}, function(scriptDoc, isLastScript) {
        findAndCheckIfFinal(times, {scriptName: scriptDoc.scriptName}, function(timeDoc, isLastTime) {
            scores.push({
                time       : timeDoc.time,
                scriptName : timeDoc.scriptName,
                levelName  : scriptDoc.levelName
            });

            if (isLastScript && isLastTime) {
                res.render('profile', { "scores": scores });
            }
        });
    });
});

// Calls BODY for each document in COLLECTION that matches FILTER.
// where BODY takes a document and a flag indicating if this is the final document.
function findAndCheckIfFinal(collection, filter, body) {
    collection.find(filter, {}, function(e, docs) {
        for (var i = 0; i < docs.length; i++) {
            body(docs[i], i == docs.length - 1);
        }
    });
}

function isLoggedIn(req, res, next) {

  req.session.redirect = '/profile';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
