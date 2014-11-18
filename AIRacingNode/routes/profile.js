var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', isLoggedIn, function(req, res) {
    var collection = req.db.get('scriptcollection');

    collection.find({email : req.user.local.email}, {sort : { scriptName : 1 }}, function(e, docs) {
      if (req.session.submitted) {
        req.session.submitted = false;
        res.render('profile', {
            "scripts" : docs,
            "submitted" : true
        });
      }
      else {
        res.render('profile', {
            "scripts" : docs,
            "submitted" : false
        });
      }
    });
});

function isLoggedIn(req, res, next) {

  req.session.redirect = '/profile';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
