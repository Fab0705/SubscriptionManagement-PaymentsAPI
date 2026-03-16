using SubscriptionManagement_PaymentsAPI.Application.TodoItems.Commands.CreateTodoItem;
using SubscriptionManagement_PaymentsAPI.Application.TodoItems.Commands.DeleteTodoItem;
using SubscriptionManagement_PaymentsAPI.Application.TodoLists.Commands.CreateTodoList;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;

using static SubscriptionManagement_PaymentsAPI.Application.FunctionalTests.Testing;

namespace SubscriptionManagement_PaymentsAPI.Application.FunctionalTests.TodoItems.Commands;

public class DeleteTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand(Guid.NewGuid());

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldDeleteTodoItem()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        await SendAsync(new DeleteTodoItemCommand(itemId));

        var item = await FindAsync<TodoItem>(itemId);

        item.ShouldBeNull();
    }
}
