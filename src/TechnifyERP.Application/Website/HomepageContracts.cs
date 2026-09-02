namespace TechnifyERP.Application.Website;

public sealed record HomepageContentDto(string HeroTitle, string HeroSubtitle, string PrimaryButtonText, string? PrimaryButtonUrl, string FooterText, IReadOnlyList<HomepageStatDto> Stats, IReadOnlyList<HomepageFeatureDto> Features);
public sealed record HomepageStatDto(string Label, string Value, int DisplayOrder);
public sealed record HomepageFeatureDto(string Title, string Description, int DisplayOrder);
public interface IHomepageService { Task<HomepageContentDto> GetAsync(CancellationToken ct = default); Task SaveAsync(HomepageContentDto content, CancellationToken ct = default); Task SaveSectionsAsync(IReadOnlyList<HomepageStatDto> stats, IReadOnlyList<HomepageFeatureDto> features, CancellationToken ct = default); }
