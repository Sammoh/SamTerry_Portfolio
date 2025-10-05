# Discord Bot Commands Reference

## Command Overview

This Discord bot includes 2 slash commands that demonstrate modern Discord bot development with interactive features.

---

## `/test` - Bot Status Test

### Description
Verify that the bot is online and working correctly. Returns a rich embed with bot and user information.

### Syntax
```
/test [message]
```

### Parameters
| Parameter | Type   | Required | Description                    |
|-----------|--------|----------|--------------------------------|
| message   | String | No       | Optional message to echo back  |

### Examples

**Basic test:**
```
/test
```

**With custom message:**
```
/test message:Hello from Discord!
```

### Response
The bot replies with a rich embed containing:
- ✅ Success confirmation
- 👤 Your username and user ID
- ⏰ Current timestamp
- 🖼️ Your Discord avatar
- 💬 Your custom message (if provided)

### Use Cases
- Verify bot is online
- Check bot response time
- Test embed functionality
- Echo messages for fun

---

## `/challenge` - User Challenge System

### Description
Challenge another Discord user to a friendly competition. The challenged user can accept or decline using interactive buttons.

### Syntax
```
/challenge <opponent> [type]
```

### Parameters
| Parameter | Type   | Required | Description                           |
|-----------|--------|----------|---------------------------------------|
| opponent  | User   | Yes      | The Discord user to challenge         |
| type      | Choice | No       | Type of challenge (default: Random)   |

### Challenge Types
| Type     | Emoji | Description                          |
|----------|-------|--------------------------------------|
| Gaming   | 🎮    | Video game competitions              |
| Trivia   | 🧩    | Knowledge-based challenges           |
| Creative | 🎨    | Art, music, or writing challenges    |
| Fitness  | 💪    | Physical activity challenges         |
| Random   | 🎲    | Any type of challenge                |

### Examples

**Challenge with default type:**
```
/challenge opponent:@JohnDoe
```

**Challenge with specific type:**
```
/challenge opponent:@JaneSmith type:Gaming
```

### Response Flow

1. **Challenge Issued** (Orange embed)
   - Shows challenger and opponent
   - Displays challenge type
   - Mentions the opponent
   - Shows Accept/Decline buttons

2. **Opponent Response** (60 seconds to respond)
   - Only the challenged user can press buttons
   - Others see "This interaction failed"

3. **Challenge Accepted** (Green embed)
   - Confirms acceptance
   - Suggests next steps
   - Removes buttons

4. **Challenge Declined** (Red embed)
   - Confirms decline
   - Removes buttons

5. **Challenge Expired** (Gray embed)
   - Shows after 60 seconds of no response
   - Removes buttons

### Validations
The bot prevents:
- ❌ Challenging yourself
- ❌ Challenging bots
- ❌ Creating duplicate active challenges

### Use Cases
- Friendly gaming competitions
- Trivia contests
- Creative challenges
- Fitness goals
- Team building activities

---

## Command Permissions

### Required Bot Permissions
- Send Messages
- Embed Links
- Read Message History
- Use Slash Commands

### User Requirements
- Must be in a server with the bot
- Must have permission to use slash commands in the channel

---

## Technical Details

### Slash Commands
All commands use Discord's native slash command system:
- Auto-complete in Discord
- Built-in validation
- Help text integration
- Mobile-friendly

### Interactive Components
The `/challenge` command uses Discord Buttons:
- Real-time interactions
- User-specific responses
- Time-limited (60 seconds)
- Automatic cleanup

### Rich Embeds
Both commands use Discord Embeds:
- Color-coded by status
- Formatted fields
- Thumbnails and images
- Timestamps

---

## Error Messages

### Common Errors

**"This interaction failed"**
- You're not the intended user for this button
- The challenge has already been responded to
- The challenge has expired

**"You cannot challenge yourself!"**
- You tried to challenge yourself
- Select a different user

**"You cannot challenge a bot!"**
- You tried to challenge a bot user
- Select a human user

**"You already have an active challenge with this user!"**
- You have an unresolved challenge with this user
- Wait for the current challenge to complete

**"There was an error executing this command!"**
- Something went wrong on the bot's side
- Check bot console for details
- Contact bot administrator

---

## Tips & Best Practices

### Using `/test`
- Use it to verify bot is responsive
- Great for testing after bot updates
- Can be used in any channel where bot has access

### Using `/challenge`
- Be respectful when challenging others
- Only challenge users who are active/online
- Coordinate follow-up in voice or text channels
- Have fun with different challenge types!

### Response Times
- Commands respond instantly (< 1 second)
- Buttons work immediately when clicked
- Challenges expire after 60 seconds

### Privacy
- All interactions are visible in the channel
- Error messages are ephemeral (private)
- Challenge details are public to the server

---

## Examples in Action

### Example 1: Quick Bot Test
```
User: /test
Bot: [Green Embed] ✅ Bot Test Successful!
     👤 Requested by: User#1234
     🆔 User ID: 123456789
     ⏰ Timestamp: March 15, 2024 10:30 AM
```

### Example 2: Gaming Challenge
```
User1: /challenge opponent:@User2 type:Gaming
Bot: @User2, you have been challenged!
     [Orange Embed] ⚔️ Challenge Issued!
     User1 has challenged User2 to a 🎮 Gaming challenge!
     [Accept Button] [Decline Button]

User2: *clicks Accept*
Bot: [Green Embed] ✅ Challenge Accepted!
     User2 has accepted the challenge from User1!
     📝 Next Steps: Coordinate with each other to complete the challenge!
```

### Example 3: Challenge Timeout
```
User1: /challenge opponent:@User2
Bot: @User2, you have been challenged!
     [Orange Embed with buttons]
     
*60 seconds pass with no response*

Bot: [Gray Embed] ⏱️ Challenge Expired
     The challenge has expired due to no response.
```

---

## Developer Notes

### Adding to Commands
To add this bot to your server:
1. Use the OAuth2 URL from Discord Developer Portal
2. Select appropriate permissions
3. Authorize for your server
4. Commands appear automatically

### Customization
Developers can:
- Modify command descriptions in code
- Add new challenge types
- Adjust timeout durations
- Create new commands following the pattern

### Command Registration
- Commands are registered on bot startup
- Global commands sync within 1 hour
- Guild commands sync instantly
- Restart bot to update commands

---

**For full setup instructions, see [README.md](README.md)**  
**For technical details, see [FEATURES.md](FEATURES.md)**  
**For quick setup, see [QUICKSTART.md](QUICKSTART.md)**
