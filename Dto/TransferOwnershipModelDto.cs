namespace testSwaggerUIError.Dto;

public class TransferOwnershipModelDto
{
    public Guid ProjectId { get; set; }
    public Guid OldOwnerId { get; set; }
    public Guid? NewOwnerId { get; set; }
    public string NewOwnerEmail { get; set; }
    public TransferOwnershipCandidateDto[] Candidates { get; set; }
}