var express = require('express');
var router = express.Router();
var coffeeScript = require('coffee-script');

router.post('/', function(req, res) {
    try {
        res.send(coffeeScript.compile(req.body.script, {bare: true}), 200);
    } catch(err) {
        res.send(err.message, 500);
    }
});

module.exports = router;
