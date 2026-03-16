using SubscriptionManagement_PaymentsAPI.Domain.Entities;

namespace SubscriptionManagement_PaymentsAPI.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto
{
    public Guid Id { get; init; }

    public Guid ListId { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TodoItem, TodoItemBriefDto>();
        }
    }
}
