<Query Kind="Expression">
  <Namespace>System.Windows.Forms</Namespace>
</Query>

new Keys[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.ControlKey, Keys.ShiftKey, Keys.O, Keys.W, Keys.C }.Select(x => new { V = x.ToString(), X = x.ToString("X") })