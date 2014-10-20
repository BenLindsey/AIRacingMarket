var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', isLoggedIn, 
  function(req, res) {
    res.render('profile', {
      user : req.user
  });
});

function isLoggedIn(req, res, next) {

  req.session.redirect = '/profile';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
