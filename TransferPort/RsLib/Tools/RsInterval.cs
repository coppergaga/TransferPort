namespace RsLib {
    public class RsInterval {
        public float interval = -1; //秒

        private float time = 0;

        public RsInterval(float interval) {
            this.interval = interval;
        }

        public bool Update(float dt) {
            if (interval < 1) {
                return true;
            }
            time += dt;
            if (time > interval) {
                time -= interval;
                return true;
            }

            return false;
        }

        public void Reset() {
            time = 0;
        }
    }
}