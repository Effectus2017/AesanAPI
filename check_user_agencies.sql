SELECT au.*, a.Name as AgencyName FROM AgencyUsers au INNER JOIN Agency a ON au.AgencyId = a.Id WHERE au.UserId = 'B03615AF-97D1-47D5-8A60-72AAF0A39ACF';
