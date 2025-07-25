using Wrok.Projects.Domain.Enums;
using Wrok.Projects.Domain.Exceptions;
using Wrok.Projects.Domain.ValueObjects;

namespace Wrok.Projects.Domain.Entities;

public sealed class Project
{
    public const int MaxAttachmentCount = 3;

    public ProjectId Id { get; private set; }
    public TenantId TenantId { get; private set; }
    public UserId ProjectManagerId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public PricingModelId PricingModelId { get; private set; }
    public ProjectTimeEstimation TimeEstimation { get; private set; }
    public ProjectStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PricingModel PricingModel { get; private set; }

    private readonly List<Attachment> _attachments;
    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

    private readonly List<Proposal> _proposals;
    public IReadOnlyCollection<Proposal> Proposals => _proposals.AsReadOnly();

#nullable disable
    private Project() { } // For EF
#nullable enable

    public Project(
        TenantId tenantId,
        UserId projectManagerId,
        string title,
        string description,
        PricingModel pricingModel,
        Money? budget,
        ProjectTimeEstimation timeEstimation)
    {
        DomainException.ThrowIfInvalidId(nameof(Project), nameof(Id), tenantId.Value);
        DomainException.ThrowIfInvalidId(nameof(Project), nameof(ProjectManagerId), projectManagerId.Value);
        DomainException.ThrowIfNullOrWhitespace(nameof(Project), nameof(Title), title);
        DomainException.ThrowIfNullOrWhitespace(nameof(Project), nameof(Description), description);

        Id = new ProjectId(Guid.CreateVersion7());
        TenantId = tenantId;
        ProjectManagerId = projectManagerId;
        Title = title;
        Description = description;
        PricingModelId = pricingModel.Id;
        PricingModel = pricingModel;
        TimeEstimation = timeEstimation;
        CreatedAt = DateTime.UtcNow;
        Status = ProjectStatus.Draft;
        _attachments = [];
        _proposals = [];
    }

    public void AddAttachment(Attachment attachment)
    {
        if (_attachments.Count >= MaxAttachmentCount)
        {
            DomainException.ThrowInvalid(nameof(Project), nameof(Attachment), $"Cannot add more than {MaxAttachmentCount} attachments to a project.");
        }
        if (_attachments.Any(a => a == attachment))
        {
            DomainException.ThrowConflict(nameof(Project), nameof(Attachment), $"Attachment already exists in project");
        }
        _attachments.Add(attachment);
    }

    public void RemoveAttachment(Attachment attachment)
    {
        if (!_attachments.Remove(attachment))
        {
            DomainException.ThrowNotFound(nameof(Project), nameof(Attachment), "Attachment not found in project.");
        }
    }

    public void Open()
    {
        Status = ProjectStatus.Open;
    }


}
