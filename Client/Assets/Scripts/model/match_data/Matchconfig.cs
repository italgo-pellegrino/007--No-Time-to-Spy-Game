public class Matchconfig { /* modified */

	/*Gadget: Moledie*/
	public int moledieRange;
	/*Gadget: BowlerBlade*/
	public int bowlerBladeRange;
	public double bowlerBladeHitChance;
	public int bowlerBladeDamage;
	/*Gadget: LaserCompact*/
	public double laserCompactHitChance;
	/*Gadget: RocketPen*/
	public int rocketPenDamage;
	/*Gadget: GasGloss*/
	public int gasGlossDamage;
	/*Gadget: MothballPouch*/
	public int mothballPouchRange;
	public int mothballPouchDamage;
	/*Gadget: FogTin*/
	public int fogTinRange;
	/*Gadget: Grapple*/
	public int grappleRange;
	public double grappleHitChance;
	/*Gadget: WiretapWithEarplugs*/
	public double wiretapWithEarplugsFailChance;
	/*Gadget: Mirror*/
	public double mirrorSwapChance;
	/*Gadget: Cocktail*/
	public double cocktailDodgeChance;
	public int cocktailHp;
	/*Aktionen*/
	public double spySuccessChance;
	public double babysitterSuccessChance;
	public double honeyTrapSuccessChance;
	public double observationSuccessChance;
	/*Spielfaktoren*/
	public int chipsToIpFactor;
	public int secretToIpFactor;
	public int minChipsRoulette;
	public int maxChipsRoulette;
	public int roundLimit;
	public int turnPhaseLimit;
	public int catIp;
	public int strikeMaximum;
	public int pauseLimit;
	public int reconnectLimit;

	public Matchconfig(int moledieRange, int bowlerBladeRange, double bowlerBladeHitChance, int bowlerBladeDamage,
	double laserCompactHitChance, int rocketPenDamage, int gasGlossDamage, int mothballPouchRange,
	int mothballPouchDamage, int fogTinRange, int grappleRange, double grappleHitChance,
	double wiretapWithEarplugsFailChance, double mirrorSwapChance, double cocktailDodgeChance, int cocktailHp,
	double spySuccessChance, double babysitterSuccessChance, double honeyTrapSuccessChance, double observationSuccessChance,
	int chipsToIpFaktor, int secretToIpFactor, int minChipsRoulette, int maxChipsRoulette, int roundLimit, int turnPhaseLimit,
	int catIp, int strikeMaximum, int pauseLimit, int reconnectLimit)
	{
		this.moledieRange = moledieRange;
		this.bowlerBladeRange = bowlerBladeRange;
		this.bowlerBladeHitChance = bowlerBladeHitChance;
		this.bowlerBladeDamage = bowlerBladeDamage;
		this.laserCompactHitChance = laserCompactHitChance;
		this.rocketPenDamage = rocketPenDamage;
		this.gasGlossDamage = gasGlossDamage;
		this.mothballPouchRange = mothballPouchRange;
		this.mothballPouchDamage = mothballPouchDamage;
		this.fogTinRange = fogTinRange;
		this.grappleRange = grappleRange;
		this.grappleHitChance = grappleHitChance;
		this.wiretapWithEarplugsFailChance = wiretapWithEarplugsFailChance;
		this.mirrorSwapChance = mirrorSwapChance;
		this.cocktailDodgeChance = cocktailDodgeChance;
		this.cocktailHp = cocktailHp;
		this.spySuccessChance = spySuccessChance;
		this.babysitterSuccessChance = babysitterSuccessChance;
		this.honeyTrapSuccessChance = honeyTrapSuccessChance;
		this.observationSuccessChance = observationSuccessChance;
		this.chipsToIpFactor = chipsToIpFaktor;
		this.secretToIpFactor = secretToIpFactor;
		this.minChipsRoulette = minChipsRoulette;
		this.maxChipsRoulette = maxChipsRoulette;
		this.roundLimit = roundLimit;
		this.turnPhaseLimit = turnPhaseLimit;
		this.catIp = catIp;
		this.strikeMaximum = strikeMaximum;
		this.pauseLimit = pauseLimit;
		this.reconnectLimit = reconnectLimit;
	}

}
