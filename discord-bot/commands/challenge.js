import { SlashCommandBuilder, EmbedBuilder, ActionRowBuilder, ButtonBuilder, ButtonStyle } from 'discord.js';

// Store active challenges in memory (for simplicity, in production use a database)
const activeChallenges = new Map();

export default {
  data: new SlashCommandBuilder()
    .setName('challenge')
    .setDescription('Challenge another user to a friendly competition')
    .addUserOption(option =>
      option
        .setName('opponent')
        .setDescription('The user you want to challenge')
        .setRequired(true)
    )
    .addStringOption(option =>
      option
        .setName('type')
        .setDescription('Type of challenge')
        .setRequired(false)
        .addChoices(
          { name: '🎮 Gaming', value: 'gaming' },
          { name: '🧩 Trivia', value: 'trivia' },
          { name: '🎨 Creative', value: 'creative' },
          { name: '💪 Fitness', value: 'fitness' },
          { name: '🎲 Random', value: 'random' }
        )
    ),

  async execute(interaction) {
    const challenger = interaction.user;
    const opponent = interaction.options.getUser('opponent');
    const challengeType = interaction.options.getString('type') || 'random';

    // Validate challenge
    if (opponent.id === challenger.id) {
      return await interaction.reply({
        content: '❌ You cannot challenge yourself!',
        ephemeral: true
      });
    }

    if (opponent.bot) {
      return await interaction.reply({
        content: '❌ You cannot challenge a bot!',
        ephemeral: true
      });
    }

    // Check for existing challenge
    const challengeKey = `${challenger.id}-${opponent.id}`;
    if (activeChallenges.has(challengeKey)) {
      return await interaction.reply({
        content: '❌ You already have an active challenge with this user!',
        ephemeral: true
      });
    }

    // Create challenge embed
    const challengeEmbed = new EmbedBuilder()
      .setColor('#ff9900')
      .setTitle('⚔️ Challenge Issued!')
      .setDescription(`${challenger} has challenged ${opponent} to a ${getChallengeTypeName(challengeType)} challenge!`)
      .addFields(
        { name: '👤 Challenger', value: challenger.tag, inline: true },
        { name: '🎯 Opponent', value: opponent.tag, inline: true },
        { name: '🎲 Challenge Type', value: getChallengeTypeName(challengeType), inline: true }
      )
      .setTimestamp()
      .setFooter({ text: 'Click a button to respond to the challenge' });

    // Create action buttons
    const row = new ActionRowBuilder()
      .addComponents(
        new ButtonBuilder()
          .setCustomId(`accept-${challengeKey}`)
          .setLabel('✅ Accept')
          .setStyle(ButtonStyle.Success),
        new ButtonBuilder()
          .setCustomId(`decline-${challengeKey}`)
          .setLabel('❌ Decline')
          .setStyle(ButtonStyle.Danger)
      );

    // Send challenge
    const response = await interaction.reply({
      content: `${opponent}, you have been challenged!`,
      embeds: [challengeEmbed],
      components: [row],
      fetchReply: true
    });

    // Store challenge info
    activeChallenges.set(challengeKey, {
      challenger: challenger.id,
      opponent: opponent.id,
      type: challengeType,
      timestamp: Date.now(),
      messageId: response.id
    });

    // Set up button interaction collector
    const collector = response.createMessageComponentCollector({
      filter: i => i.user.id === opponent.id,
      time: 60000 // 1 minute timeout
    });

    collector.on('collect', async i => {
      const action = i.customId.split('-')[0];
      
      if (action === 'accept') {
        const acceptEmbed = new EmbedBuilder()
          .setColor('#00ff00')
          .setTitle('✅ Challenge Accepted!')
          .setDescription(`${opponent} has accepted the challenge from ${challenger}!`)
          .addFields(
            { name: '🎲 Challenge Type', value: getChallengeTypeName(challengeType), inline: false },
            { name: '📝 Next Steps', value: 'Coordinate with each other to complete the challenge!', inline: false }
          )
          .setTimestamp();

        await i.update({ embeds: [acceptEmbed], components: [] });
        activeChallenges.delete(challengeKey);
      } else if (action === 'decline') {
        const declineEmbed = new EmbedBuilder()
          .setColor('#ff0000')
          .setTitle('❌ Challenge Declined')
          .setDescription(`${opponent} has declined the challenge from ${challenger}.`)
          .setTimestamp();

        await i.update({ embeds: [declineEmbed], components: [] });
        activeChallenges.delete(challengeKey);
      }
    });

    collector.on('end', async (collected) => {
      if (collected.size === 0) {
        const timeoutEmbed = new EmbedBuilder()
          .setColor('#808080')
          .setTitle('⏱️ Challenge Expired')
          .setDescription('The challenge has expired due to no response.')
          .setTimestamp();

        try {
          await response.edit({ embeds: [timeoutEmbed], components: [] });
        } catch (error) {
          console.error('Failed to update expired challenge:', error);
        }
        
        activeChallenges.delete(challengeKey);
      }
    });
  },
};

function getChallengeTypeName(type) {
  const types = {
    'gaming': '🎮 Gaming',
    'trivia': '🧩 Trivia',
    'creative': '🎨 Creative',
    'fitness': '💪 Fitness',
    'random': '🎲 Random'
  };
  return types[type] || '🎲 Random';
}
