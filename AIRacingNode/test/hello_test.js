var Browser = require("zombie");
var assert = require("assert");

browser = new Browser();
browser.visit("http://localhost:3013/login", function () {

  browser.
    fill("email", "a@a.com").
    fill("password", "a").
    pressButton("Login", function() {
      assert.ok(browser.success);
      assert.equal(browser.text("title"), "AI Racing Market");
    })

});
