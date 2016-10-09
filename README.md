## Summary

This repository is used in the blog post digging database transaction internals below.

[www.gokhansengun.com/why-do-long-db-transactions-affect-performance](www.gokhansengun.com/why-do-long-db-transactions-affect-performance)

## Excerpt from the blog post

So almost everybody working with database transactions knows that the transactions should be started and then committed as soon as we are done with it. Is this really the case? If this is the case, what is the underlying mechanism causing performance issues if we start a transaction and do not hurry up in committing it or our business just does not allow to commit it very quickly? In this blog post, we will try to dig this subject by using a few programs.

### Work Summary

In this blog, we will try to see mechanics of database transaction management and try to prove that the database transactions should be committed as quickly as possible in order not to lack performance. 

* We will first introduce what a database transaction is.
* We will investigate how an application handles a transaction with the database server.
* We will learn what connection pooling is.
* We will write a program to better see the mechanics of the database transaction handling of an application.
* We will alter the program to show performance hit this time if long running database transactions are used.
* We will talk about whether ambient transactions behave
* We will finally talk about the countermeasures to alleviate performance issues if we just really can not commit quickly due to the business logic we are running. 
