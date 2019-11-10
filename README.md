# Web API prototype project
This repository stores a REST API project. It is a small prototype that is similar to a real project, that will be relevant for the back-end position.

Below is a few tasks that we have prepared for you. We only expect you to spend around 3 hours on this – not days. The most important is for us to get insight into your understanding/thinking. We ask you to do the following:

1. Fork this repo to your own GitHub account and clone your fork to your machine. Run the application and get an overview over how it is working.
2. Review the code base and think about how it could be improved – especially the general structure and code patterns.
3. Choose to do some relevant changes, accompanied by inline comments explaining the change and why.
4. More overall thoughts/suggestions/ideas for the code or architecture should be added below in this README. This also gives you a chance to mention changes that you did not have time to do in the short timeframe.
5. Push and make a pull request to this repository with your changes.

----

### Add general thoughts/suggestions/ideas here:

#### Split application models and DTOs completely
The models in the `DomainModels` namespace seem to be DTOs that map directly to the data model. These models are used directly in the application and used as return types in the controllers. The one exception being the `ExtendedBook` class which is only used in a single controller action. 

This means that any change to the datamodel will be reflected in the API output and input and vice versa that any change to the application input and output _has_ to be reflected in the datamodel.

Instead I would make seperate models for the application layer to avoid having any direct dependencies between the application and the datamodel. The logic for conversion between the models could be handled in factories called from the service layer, i.e. `BookService` and `UserService`.

#### Rethink the project structure
I think the project structure is a bit wonky as it groups by type. This causes some potential issue:
1. Interfaces are in a separate project from the types they work with (the models) creating a "hidden" dependency to the model project for all projects that need to reference the Interface project.
2. Due to DTOs and application models being grouped together every project in the solution reference the Models project although they only need to work with either DTOs or application models.
3. The dependency hierarchy between the projects is not obvious from the structure which may lead to issues with dependency cycles if the solution grows significantly in size.

I would restructure the solution to use something closer to an n-tier architecture with the following tiers:
- Presentation (the BookApp project)
- Logic (domain level models, services, interfaces for services)
- Data (DTOs, repositories, interfaces for repositories)

#### Minor things
The `BookService.GetBooksByUserId` seem to have some weird logic. It retrieves all books for a user, and then for each these books it adds all the other books of the user as `MoreUserBooks` of that book. This seems a bit odd to have list of books in which each book reference all other books in the list.

The `UserController` reference the `BookRepository` although the `BookService` expose all the methods it require (I changed this in the repo).

