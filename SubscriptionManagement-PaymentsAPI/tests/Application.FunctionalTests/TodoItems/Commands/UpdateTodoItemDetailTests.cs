using SubscriptionManagement_PaymentsAPI.Application.TodoItems.Commands.CreateTodoItem;
using SubscriptionManagement_PaymentsAPI.Application.TodoItems.Commands.UpdateTodoItem;
using SubscriptionManagement_PaymentsAPI.Application.TodoItems.Commands.UpdateTodoItemDetail;
using SubscriptionManagement_PaymentsAPI.Application.TodoLists.Commands.CreateTodoList;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;

using static SubscriptionManagement_PaymentsAPI.Application.FunctionalTests.Testing;

namespace SubscriptionManagement_PaymentsAPI.Application.FunctionalTests.TodoItems.Commands;

public class UpdateTodoItemDetailTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = Guid.NewGuid(), Title = "New Title" };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = listId,
            Note = "This is the note.",
            Priority = PriorityLevel.High
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.ListId.ShouldBe(command.ListId);
        item.Note.ShouldBe(command.Note);
        item.Priority.ShouldBe(command.Priority);
        item.LastModifiedBy.ShouldNotBeNull();
        item.LastModifiedBy.ShouldBe(userId.ToString());
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
