var express = require('express');
var router = express.Router();
var User = require('../models/user');

/* GET user profile. */
router.get('/', function(req, res) {
    var collection = req.db.get('scriptcollection');

    collection.find({email : req.user.local.email}, {sort : { scriptName : 1 }}, function(e, docs) {
      var submitted_recently = req.session.submitted;
      req.session.submitted = false;
      res.render('profile', {
          "scripts"   : docs,
          "submitted" : submitted_recently,
          "userYear"  : req.user.local.year,
          "userEmail" : req.user.local.email,
          "userUni"   : req.user.local.university,
          "userDeg"   : req.user.local.degree
      });
    });
});

/* POST updated user details. */

router.post('/', function(req, res) {

  User.findOne({ "local.email" : req.user.local.email}, function(err, user) {
    console.log("Handling results of user search...");
    if (err) {
        console.log("Error finding user " + err);
        res.send("There was a problem finding your information to the database.");
        return done(err);
    }
    if (user) {
      console.log("No errors finding user " + user);
      //user.email  = req.body.emailaddress;
      user.local.university = req.body.university;
      user.local.degree     = req.body.course;
      user.local.year       = req.body.year;

      user.save(function(err) {
        if (err) {
          console.log("Failed to save changes " + err);
          return done(err);
        }

        req.login(user, function(err) {
          if (err) {
            console.log("Failed to save changes " + err);
            return done(err);
          }
          console.log("Saved changes.");
        });
      });
    }
    else {
      console.log("No user found");
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
