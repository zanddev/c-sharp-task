// Not implemented
using Graphics2D;

import java.awt.Color;
import java.util.HashMap;
import java.util.Map;
import java.util.Random;
import java.util.Set;

// Not implemented here
using game.frame.GameWindow;
using game.logics.interactions.SpeedHandler;
using game.utility.debug.Debugger;

namespace JetScape.Background
{
    public class BackgroundController : Background
    {

        private static readonly string SpritePath = "background" + System.getProperty("file.separator");

        private static readonly Color PlaceHolder = Color.getHSBColor((float) 0.666, (float) 0.333, (float) 0.141);

        private const readonly string KeySprite1 = "background1";

        private const readonly string KeySprite2 = "background2";

        private static readonly double LadderGeneration = 0.15;

        private static readonly int ScreenWidth = GameWindow.GameScreen.GetWidth();

        private static readonly (double X, double Y) StartPosition = (0.0, 0.0);

        private readonly (double X, double Y) position = StartPosition.Copy();

        private readonly IBackgroundDrawer _drawMgr = new BackgroundDrawManager();
        private readonly var _rand = new Random();
        private readonly SpeedHandler _movement;

        /// FLAGS ///
        private bool _visible;
        private bool _toBeGenerated;
        private bool _toBeShifted;
        private readonly IDictionary<BoxPos, bool> _boxVisible;
        private readonly IDictionary<BoxPos, string?> _boxSprite;


        public BackgroundController(SpeedHandler speed) {

            _movement = speed;
            _drawMgr.SetPlaceH(PlaceHolder);
            _drawMgr.AddSprite(KeySprite1, SpritePath + "background_1.png");
            _drawMgr.AddSprite(KeySprite2, SpritePath + "background_2.png");

            _boxVisible = new Dictionary<BoxPos, bool>() {
                {BoxPos.LEFT, false},
                {BoxPos.CENTRAL, true},
                {BoxPos.RIGHT, false}
            };
            _boxSprite = new Dictionary<BoxPos, string?>(3);

		    foreach (BoxPos pos in BoxPos.GetValues())
            {
                _boxSprite.Add(pos, null));
            }

            _boxSprite.Add(BoxPos.CENTRAL, KeySprite1?);
            _boxSprite.Add(BoxPos.RIGHT, KeySprite1?);

            SetVisibility(true);
        }


        public sealed void SetVisibility(bool v)
        {
            _visible = v;
        }

        public sealed bool IsVisible()
        {
            return _visible;
        }

        public void Reset()
        {
            _position.Set(StartPosition.GetX(), StartPosition.GetY());
            _movement.ResetSpeed();
        }

        public void update() {
            UpdateFlags();
            if (IsVisible()) {
            if (this.toBeGenerated) {
                this.boxSprite.put(BoxPos.RIGHT,
                        rand.nextDouble() > BackgroundController.LADDER_GENERATION
                        ? Optional.of(BackgroundController.KEY_SPRITE1)
                        : Optional.of(BackgroundController.KEY_SPRITE2));
            }
            if (this.toBeShifted) {
                this.shiftBox();
                this.toBeShifted = false;
            }
            if (this.position.getX() > -SCREEN_WIDTH * 2) {
                this.position.setX(this.position.getX() - this.movement.getXSpeed() / GameWindow.FPS_LIMIT);
            }
        }
    }

    public void draw(final Graphics2D g) {
        if (this.isVisible()) {
            this.boxSprite.entrySet().stream()
                    .filter(box -> this.boxVisible.get(box.getKey()))
                    .forEach(box -> this.drawMgr.drawSprite(g,
                            box.getValue().orElse(BackgroundDrawer.PLACEHOLDER_KEY),
                            this.calculate(box.getKey()),
                            GameWindow.GAME_SCREEN.getHeight(),
                            GameWindow.GAME_SCREEN.getWidth()));
        }
    }

    private void shiftBox() {

        this.boxVisible.putAll(new HashMap<>(Map.of(
                BoxPos.LEFT, true,
                BoxPos.CENTRAL, true,
                BoxPos.RIGHT, false)));

        this.boxSprite.put(BoxPos.LEFT, this.boxSprite.get(BoxPos.CENTRAL));
        this.boxSprite.put(BoxPos.CENTRAL, this.boxSprite.get(BoxPos.RIGHT));

        final Pair<Double, Double> temp = this.calculate(BoxPos.RIGHT);
        this.position.set(temp.getX(), temp.getY());
    }

    private Pair<Double, Double> calculate(final BoxPos box) {
        final Pair<Double, Double> newPos;
        switch (box) {
            case LEFT:
                newPos = new Pair<>(this.position.getX() - SCREEN_WIDTH, this.position.getY());
                break;
            case RIGHT:
                newPos = new Pair<>(this.position.getX() + SCREEN_WIDTH, this.position.getY());
                break;
            default:
                //newPos = new Pair<>(this.position);
                newPos = this.position.copy();
                break;
        }
        return newPos;
    }

    public void drawCoordinates(final Graphics2D g) {
        final int xShift = (int) Math.round(position.getX())
                + (int) Math.round(GameWindow.GAME_SCREEN.getTileSize() * 0.88);
        final int yShiftDrawnX = (int) Math.round(position.getY())
                + GameWindow.GAME_SCREEN.getTileSize();
        final int yShiftDrawnY = yShiftDrawnX + 10;

        if (GameWindow.GAME_DEBUGGER.isFeatureEnabled(Debugger.Option.BACKGROUND_COORDINATES) && this.isVisible()) {
            g.setColor(Debugger.DEBUG_COLOR);
            g.setFont(Debugger.DEBUG_FONT);

            g.drawString("X:" + Math.round(position.getX()), xShift, yShiftDrawnX);
            g.drawString("Y:" + Math.round(position.getY()), xShift, yShiftDrawnY);
        }
    }

    private void updateFlags() {
        if (position.getX() <= 0) {
            toBeGenerated = true;
            toBeShifted = true;
        } else {
            toBeGenerated = false;
        }
    }

    @Override
    public String toString() {
        return "Background"
                + "[L: X:" + Math.round(this.calculate(BoxPos.LEFT).getX())
                +  " - Y:" + Math.round(this.calculate(BoxPos.LEFT).getY()) + "]\n" + "           "
                + "[C: X:" + Math.round(position.getX())
                +  " - Y:" + Math.round(position.getY()) + "]\n" + "           "
                + "[R: X:" + Math.round(this.calculate(BoxPos.RIGHT).getX())
                +  " - Y:" + Math.round(this.calculate(BoxPos.RIGHT).getY()) + "]";
    }

        private enum BoxPos
        {
            LEFT, CENTRAL, RIGHT;
        }
    }
}
