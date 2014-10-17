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

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
