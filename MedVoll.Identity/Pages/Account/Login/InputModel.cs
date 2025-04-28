// Copyright (c) Duende Software. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace MedVoll.Identity.Pages.Login;

public class InputModel
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
    public bool RememberLogin { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Button { get; set; }
}
