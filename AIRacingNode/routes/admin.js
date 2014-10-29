var express = require('express');
var router = express.Router();

/* GET tournament form */
router.get('/', isLoggedIn, function(req, res) {
    var collection = req.db.get('scriptcollection');

    collection.find({}, {sort : { scriptName : 1 }}, function(e, docs) {
        res.render('admin', {
            "scripts" : docs
        });
    });
});

/* POST to start tournament */
router.post('/', isLoggedIn, function(req, res) {
    // Build the inputs to unity
    var url = "/WebBuild.html?scriptnameA=" + req.body.scriptnameA
        +"&scriptnameB=" + req.body.scriptnameB;
        +"&scriptnameC=" + req.body.scriptnameC;
        +"&scriptnameD=" + req.body.scriptnameD;
        +"&levelname=" + req.body.levelname;

    console.log("Redirecting user to: " + url);
    // If it worked, set the header so the address bar doesn't still say /script
    res.location(url);
    // And forward to success page
    res.redirect(url);
});

module.exports = router;
