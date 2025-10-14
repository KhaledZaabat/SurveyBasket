


namespace SurveyBasket.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateSurveyRequest, Survey>()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Summary, src => src.Summary)
            .Map(dest => dest.Status, src => new PublishStatus(src.IsPublished))
            .Map(dest => dest.StartsAt, src => src.StartsAt)
            .Map(dest => dest.EndsAt, src => src.EndsAt)
            .TwoWays();

        config.NewConfig<Survey, SurveyResponse>()
            .Map(des => des.IsPublished, src => src.Status.IsPublished);



        config.NewConfig<UpdateSurveyRequest, Survey>()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Summary, src => src.Summary)
            .Ignore(dest => dest.Status)
            .Map(dest => dest.StartsAt, src => src.StartsAt)
            .Map(dest => dest.EndsAt, src => src.EndsAt)
            .TwoWays();

        config.NewConfig<SurveyOptionResponse, SurveyOption>()
            .Map(des => des.Content, src => src.Content)
            .Map(des => des.Id, src => src.Id)
            .Ignore(d => d.CreatedBy)
            .Ignore(d => d.CreatedById)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.DeletedBy)
            .Ignore(d => d.DeletedById)
            .Ignore(d => d.DeletedOn)
            .Ignore(d => d.IsDeleted)
            .Ignore(d => d.SurveyQuestionId)
            .Ignore(d => d.UpdatedBy)
            .Ignore(d => d.UpdatedById)
            .Ignore(d => d.UpdatedOn).TwoWays();


        config.NewConfig<SurveyQuestion, SurveyQuestionResponse>()
            .Map(des => des.Content, src => src.Content)
            .Map(des => des.Id, src => src.Id)
            .Map(des => des.AnswerResponses, src => src.SurveyOptions)
            .TwoWays();
        config.NewConfig<CreateSurveyQuestionRequest, SurveyQuestion>()
              .Map(dest => dest.SurveyOptions,
                src => src.SurveyQuestions.Select(a => new SurveyOption { Content = a }).ToList());
        config.NewConfig<UpdateSurveyQuestionRequest, SurveyQuestion>()
              .Map(dest => dest.SurveyOptions,
                src => src.SurveyQuestions.Select(a => new SurveyOption { Content = a }).ToList());



        config.NewConfig<SubmissionDetailRequest, SubmissionDetail>()
            .Map(des => des.OptionId, src => src.OptionId)
            .Map(des => des.QuestionId, src => src.QuestionId).TwoWays();
        config.NewConfig<UserSubmissionRequest, UserSubmission>()
            .Map(des => des.SubmissionDetails, src => src.submissionDetails);


        config.NewConfig<SubmissionDetail, QuestionOptionResponse>()
            .Map(des => des.Answer, src => src.OptionId)
            .Map(des => des.Question, src => src.QuestionId);



        config.NewConfig<Survey, SurveySubmissionsResponse>()
            .Map(des => des.Title, src => src.Title)
            .Map(des => des.Submissions, src => src.CreatedBy.UserSubmissions);

        config.NewConfig<UserSubmission, SubmissionResponse>()
         .Map(des => des.SubmittedDate, src => src.SubmittedOn)
         .Map(des => des.SubmitterName, src => $"{src.User.FirstName} {src.User.LastName}")
         .Map(des => des.SelectedAnswers, src => src.SubmissionDetails);

        config.NewConfig<List<UserSubmission>, SurveySubmissionsResponse>()
            .Map(des => des.Title, src => src.FirstOrDefault()!.Survey.Title)
   .Map(des => des.Submissions, src => src.Adapt<List<SubmissionResponse>>());

    }
}


